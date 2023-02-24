using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Acidmanic.Utilities.Results;

namespace EnTier.EventStreams;

public interface IEventStreamRepository<TEvent,TEventId,TStreamId>
{

    Task<Result<TEventId>> Append(TEvent ev,TStreamId streamId);

    Task<IEnumerable<TEvent>> ReadStream();
    
    Task<IEnumerable<TEvent>> ReadStream(TStreamId streamId);

    Task<IEnumerable<TEvent>> ReadStream(TStreamId streamId,TEventId baseEventId);

    Task<Dictionary<TStreamId, List<TEvent>>> ReadStreamsGrouped();
    
    Task<IEnumerable<TEvent>> ReadStreamChunk(TStreamId streamId,TEventId baseEventId,long count);
    
    Task<IEnumerable<TEvent>> ReadStreamChunk(TEventId baseEventId,long count);
    
    Task EnumerateStreamChunks(TStreamId streamId, Action<IEnumerable<TEvent>> process, long chunkSize = 50);
    
    Task EnumerateChunks(Action<IEnumerable<TEvent>> process, long chunkSize = 50);
    
    
    
}