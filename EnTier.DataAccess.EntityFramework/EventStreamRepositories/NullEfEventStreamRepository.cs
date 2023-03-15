using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Acidmanic.Utilities.Results;
using EnTier.Repositories;
using EnTier.Repositories.Models;

namespace EnTier.DataAccess.EntityFramework.EventStreamRepositories;

internal class NullEfEventStreamRepository
{
}
internal class NullEfEventStreamRepository<TEvent, TEventId, TStreamId>
    : NullEfEventStreamRepository ,IEventStreamRepository<TEvent, TEventId, TStreamId>
{
    private static readonly object Lock = new object();

    private static NullEfEventStreamRepository<TEvent, TEventId, TStreamId> _instance = null;

    public static NullEfEventStreamRepository<TEvent, TEventId, TStreamId> Instance
    {
        get
        {
            lock (Lock)
            {
                if (_instance == null)
                {
                    _instance = new NullEfEventStreamRepository<TEvent, TEventId, TStreamId>();
                }

                return _instance;
            }
        }
    }

    private NullEfEventStreamRepository()
    {
    }

    public Task<Result<TEventId>> Append(TEvent ev, TStreamId streamId)
    {
        return Task.FromResult(new Result<TEventId>().FailAndDefaultValue());
    }

    public Task<IEnumerable<TEvent>> ReadStream()
    {
        return Task.FromResult<IEnumerable<TEvent>>(new List<TEvent>());
    }

    public Task<IEnumerable<TEvent>> ReadStream(TStreamId streamId)
    {
        return Task.FromResult<IEnumerable<TEvent>>(new List<TEvent>());
    }

    public Task<IEnumerable<TEvent>> ReadStream(TStreamId streamId, TEventId baseEventId)
    {
        return Task.FromResult<IEnumerable<TEvent>>(new List<TEvent>());
    }

    public Task<IEnumerable<TEvent>> ReadStreamChunk(TStreamId streamId, TEventId baseEventId, long count)
    {
        return Task.FromResult<IEnumerable<TEvent>>(new List<TEvent>());
    }

    public Task<IEnumerable<TEvent>> ReadStreamChunk(TEventId baseEventId, long count)
    {
        return Task.FromResult<IEnumerable<TEvent>>(new List<TEvent>());
    }

    public Task EnumerateStreamChunks(TStreamId streamId,
        Action<IEnumerable<StreamEvent<TEvent, TEventId, TStreamId>>> process, long chunkSize = 50)
    {
        return Task.CompletedTask;
    }

    public Task EnumerateChunks(Action<IEnumerable<StreamEvent<TEvent, TEventId, TStreamId>>> process,
        long chunkSize = 50)
    {
        return Task.CompletedTask;
    }
}