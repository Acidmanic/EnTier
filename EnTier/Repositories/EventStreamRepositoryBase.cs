using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Acidmanic.Utilities.Results;
using EnTier.Repositories.Models;
using Newtonsoft.Json;

namespace EnTier.Repositories;

public abstract class EventStreamRepositoryBase<TEvent, TEventId, TStreamId> :
    IEventStreamRepository<TEvent, TEventId, TStreamId>
{
    private readonly Action<TEvent, TEventId> _eventPublisher;

    protected EventStreamRepositoryBase() : this((e, i) => { })
    {
    }

    protected EventStreamRepositoryBase(Action<TEvent, TEventId> eventPublisher)
    {
        _eventPublisher = eventPublisher;
    }

    private string SerialiseValue(TEvent value)
    {
        string json = JsonConvert.SerializeObject(value);

        byte[] jsonData = Encoding.Unicode.GetBytes(json);

        string serialized = Convert.ToBase64String(jsonData)
            .Replace(" ", "")
            .Replace("\t", "")
            .Replace("\r", "")
            .Replace("\n", "");

        return serialized;
    }

    private TEvent DeSerialiseValue(string serialised, string typeName)
    {
        try
        {
            var jsonData = Convert.FromBase64String(serialised);

            var json = Encoding.Unicode.GetString(jsonData);

            var type = Type.GetType(typeName);

            if (type != null)
            {
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

                var typeName =type.AssemblyQualifiedName ??  type.FullName ?? type.Name;

                var serializedValue = SerialiseValue(ev);

                var entry = new ObjectEntry<TEventId, TStreamId>(default, streamId, typeName, serializedValue);

                var eventId = AppendEntry(entry);

                TryPublish(ev, eventId);

                return new Result<TEventId>(true, eventId);
            }

            return new Result<TEventId>().FailAndDefaultValue();
        });
        task.Start();
        return task;
    }


    private void TryPublish(TEvent @event, TEventId eventId)
    {
        try
        {
            _eventPublisher(@event, eventId);
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