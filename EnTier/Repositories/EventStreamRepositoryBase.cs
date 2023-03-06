using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Acidmanic.Utilities;
using Acidmanic.Utilities.Results;
using EnTier.Repositories.Attributes;
using EnTier.Repositories.Models;
using Newtonsoft.Json;

namespace EnTier.Repositories;

public abstract class EventStreamRepositoryBase<TEvent, TEventId, TStreamId> :
    IEventStreamRepository<TEvent, TEventId, TStreamId>
{
    private readonly Action<TEvent, TEventId, TStreamId> _eventPublisher;

    private readonly Func<string, string> _stringPacker;
    private readonly Func<string, string> _stringUnPacker;


    protected EventStreamRepositoryBase() : this((e, i, s) => { })
    {
    }

    protected EventStreamRepositoryBase(Action<TEvent, TEventId, TStreamId> eventPublisher)
    {
        _eventPublisher = eventPublisher;

        _stringPacker = GetPackingFunction(true);
        _stringUnPacker = GetPackingFunction(false);
    }

    protected EventStreamRepositoryBase(EnTierEssence essence)
    {
        _eventPublisher = (e, i, s) => essence.StreamEventPublisherAdapter.Publish(e, i, s);
        _stringPacker = GetPackingFunction(true);
        _stringUnPacker = GetPackingFunction(false);
    }

    private string SerialiseValue(TEvent value)
    {
        string json = JsonConvert.SerializeObject(value);

        var serialized = _stringPacker(json);

        return serialized;
    }

    private TEvent DeSerialiseValue(string serialised, string typeName)
    {
        try
        {
            var type = Type.GetType(typeName);

            if (type != null)
            {
                var json = _stringUnPacker(serialised);
                
                var objectValue = JsonConvert.DeserializeObject(json, type);

                if (objectValue is TEvent tValue)
                {
                    return tValue;
                }
            }
        }
        catch (Exception _)
        {
        }

        return default;
    }


    private Func<string, string> Base64Conversion(bool toBase64)
    {
        if (toBase64)
        {
            return value =>
            {
                byte[] data = Encoding.Unicode.GetBytes(value);

                return Convert.ToBase64String(data)
                    .Replace(" ", "")
                    .Replace("\t", "")
                    .Replace("\r", "")
                    .Replace("\n", "");
            };
        }

        return value =>
        {
            var data = Convert.FromBase64String(value);

            return Encoding.Unicode.GetString(data);
        };
    }


    private Func<string, string> GetPackingFunction(bool packer)
    {
        var eventType = typeof(TEvent);

        var eventStreamAttribute = eventType.GetCustomAttribute<EventStreamRecordCompressionAttribute>();

        if (eventStreamAttribute != null)
        {
            return packer
                ? s => s.CompressAsync(eventStreamAttribute.Compression, eventStreamAttribute.CompressionLevel).Result
                : s => s.DecompressAsync(eventStreamAttribute.Compression).Result;
        }

        return Base64Conversion(packer);
    }

    protected abstract TEventId AppendEntry(ObjectEntry<TEventId, TStreamId> entry);

    private Result<TEvent> ReadEvent(ObjectEntry<TEventId, TStreamId> entry)
    {
        var obj = DeSerialiseValue(entry.SerializedValue, entry.TypeName);

        if (obj != null)
        {
            return new Result<TEvent>(true, obj);
        }

        return new Result<TEvent>().FailAndDefaultValue();
    }


    public Task<Result<TEventId>> Append(TEvent ev, TStreamId streamId)
    {
        var task = new Task<Result<TEventId>>(() =>
        {
            if (ev != null)
            {
                var type = ev.GetType();

                var typeName = type.AssemblyQualifiedName ?? type.FullName ?? type.Name;

                var serializedValue = SerialiseValue(ev);

                var entry = new ObjectEntry<TEventId, TStreamId>(default, streamId, typeName, serializedValue);

                var eventId = AppendEntry(entry);

                TryPublish(ev, eventId, streamId);

                return new Result<TEventId>(true, eventId);
            }

            return new Result<TEventId>().FailAndDefaultValue();
        });
        task.Start();
        return task;
    }


    private void TryPublish(TEvent @event, TEventId eventId, TStreamId streamId)
    {
        try
        {
            _eventPublisher(@event, eventId, streamId);
        }
        catch (Exception e)
        {
        }
    }


    protected abstract Task<IEnumerable<ObjectEntry<TEventId, TStreamId>>> ReadAllEntries();

    protected abstract Task<IEnumerable<ObjectEntry<TEventId, TStreamId>>> ReadEntriesByStreamId(TStreamId streamId);

    protected abstract Task<IEnumerable<ObjectEntry<TEventId, TStreamId>>> ReadEntryChunk(TStreamId streamId,
        TEventId baseEventId,
        long count);

    protected abstract Task<IEnumerable<ObjectEntry<TEventId, TStreamId>>> ReadEntryChunk(TEventId baseEventId,
        long count);


    private List<TEvent> ReadEvents(IEnumerable<ObjectEntry<TEventId, TStreamId>> entries)
    {
        var events = new List<TEvent>();

        foreach (var entry in entries)
        {
            var ev = ReadEvent(entry);

            if (ev)
            {
                events.Add(ev.Value);
            }
        }

        return events;
    }

    private List<StreamEvent<TEvent, TEventId, TStreamId>> ReadStreamEvents(
        IEnumerable<ObjectEntry<TEventId, TStreamId>> entries)
    {
        var events = new List<StreamEvent<TEvent, TEventId, TStreamId>>();

        foreach (var entry in entries)
        {
            var ev = ReadEvent(entry);

            if (ev)
            {
                events.Add(new StreamEvent<TEvent, TEventId, TStreamId>
                {
                    Event = ev.Value,
                    EventId = entry.EventId,
                    StreamId = entry.StreamId
                });
            }
        }

        return events;
    }


    public async Task<IEnumerable<TEvent>> ReadStream()
    {
        var stream = await ReadAllEntries();

        var events = ReadEvents(stream);

        return events;
    }


    public async Task<IEnumerable<TEvent>> ReadStream(TStreamId streamId)
    {
        var stream = await ReadEntriesByStreamId(streamId);

        var events = ReadEvents(stream);

        return events;
    }

    public Task<IEnumerable<TEvent>> ReadStream(TStreamId streamId, TEventId baseEventId)
    {
        return ReadStreamChunk(streamId, baseEventId, long.MaxValue);
    }

    public async Task<IEnumerable<TEvent>> ReadStreamChunk(TStreamId streamId, TEventId baseEventId, long count)
    {
        var stream = await ReadEntryChunk(streamId, baseEventId, count);

        var events = ReadEvents(stream);

        return events;
    }

    public async Task<IEnumerable<TEvent>> ReadStreamChunk(TEventId baseEventId, long count)
    {
        var stream = await ReadEntryChunk(baseEventId, count);

        var events = ReadEvents(stream);

        return events;
    }


    public Task EnumerateStreamChunks(TStreamId streamId,
        Action<IEnumerable<StreamEvent<TEvent, TEventId, TStreamId>>> process, long chunkSize = 50)
    {
        return EnumerateChunksOfEntriesAsStreamChunks(eId => ReadEntryChunk(streamId, eId, chunkSize), process);
    }

    public Task EnumerateChunks(Action<IEnumerable<StreamEvent<TEvent, TEventId, TStreamId>>> process,
        long chunkSize = 50)
    {
        return EnumerateChunksOfEntriesAsStreamChunks(eId => ReadEntryChunk(eId, chunkSize), process);
    }

    private async Task EnumerateChunksOfEntriesAsStreamChunks(
        Func<TEventId, Task<IEnumerable<ObjectEntry<TEventId, TStreamId>>>> readEntryChunk,
        Action<IEnumerable<StreamEvent<TEvent, TEventId, TStreamId>>> process)
    {
        var eventsToProcess = true;

        TEventId baseEventId = default;

        while (eventsToProcess)
        {
            var entriesEnumerable = await readEntryChunk(baseEventId);

            var entries = new List<ObjectEntry<TEventId, TStreamId>>(entriesEnumerable);

            var stream = ReadStreamEvents(entries);

            if (!stream.Any())
            {
                eventsToProcess = false;
            }
            else
            {
                process(stream);

                baseEventId = entries.Last().EventId;
            }
        }
    }
}