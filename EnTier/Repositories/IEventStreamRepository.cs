using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Acidmanic.Utilities.Results;
using Microsoft.Extensions.Logging;

namespace EnTier.Repositories;

public interface IEventStreamRepository<TEvent, TEventId, TStreamId>
{
    public class StreamEvent
    {
        public TEvent Event { get; set; }
        
        public TStreamId StreamId { get; set; }
        
        public EventId EventId { get; set; }
        
    }
    /// <summary>
    /// Appends and event at the end of the event stream whom which streamId is pointing to.
    /// </summary>
    /// <param name="ev">The inserting event</param>
    /// <param name="streamId">The id of the stream which the event is being appended to.</param>
    /// <returns>
    /// Successful result containing the eventId for newly inserted event, or in case of failure,
    /// returns a failure result.
    /// </returns>
    Task<Result<TEventId>> Append(TEvent ev, TStreamId streamId);

    /// <summary>
    /// Reads all events belonging to all streams.
    /// </summary>
    /// <returns>A collection of all events from all streams.</returns>
    Task<IEnumerable<TEvent>> ReadStream();

    /// <summary>
    /// Reads all events belonging to the stream specified by streamId.
    /// </summary>
    /// <param name="streamId">The id of stream which its event are being retrieved.</param>
    /// <returns>A collection of all events from the specified stream.</returns>
    Task<IEnumerable<TEvent>> ReadStream(TStreamId streamId);

    /// <summary>
    /// Reads all events belonging to the stream specified by streamId,
    /// which are appended after the event specified by baseEventId (<b>Excluding the baseEvent itself</b>). 
    /// </summary>
    /// <param name="streamId">The id of stream which its event are being retrieved.</param>
    /// <param name="baseEventId">The id of the base event.</param>
    /// <returns>A collection of all events from the specified stream.</returns>
    Task<IEnumerable<TEvent>> ReadStream(TStreamId streamId, TEventId baseEventId);

    /// <summary>
    /// Reads limited number of events (specified by count parameter) belonging to the stream specified by streamId,
    /// which are appended after the event specified by baseEventId (<b>Excluding the baseEvent itself</b>). 
    /// </summary>
    /// <param name="streamId">The id of stream which its event are being retrieved.</param>
    /// <param name="baseEventId">The id of the base event.</param>
    /// <param name="count">Maximum number of events to be returned</param>
    /// <returns>A collection of (0 to 'count') events from the specified stream.</returns>
    Task<IEnumerable<TEvent>> ReadStreamChunk(TStreamId streamId, TEventId baseEventId, long count);
    /// <summary>
    /// Reads limited number of events (specified by count parameter) belonging any stream,
    /// which are appended after the event specified by baseEventId (<b>Excluding the baseEvent itself</b>). 
    /// </summary>
    /// <param name="baseEventId">The id of the base event.</param>
    /// <param name="count">Maximum number of events to be returned</param>
    /// <returns>A collection of (0 to 'count') events from any stream.</returns>
    Task<IEnumerable<TEvent>> ReadStreamChunk(TEventId baseEventId, long count);

    /// <summary>
    /// Enumerates all events of the stream specified by streamId, reading a limited number of events
    /// (specified by chunkSize) each time until all events are read.
    /// </summary>
    /// <param name="streamId">The id of stream which its event are being retrieved.</param>
    /// <param name="process">After each read, this method will be called to process retrieved events.</param>
    /// <param name="chunkSize">Maximum number of events to be read in each step</param>
    /// <returns>A collection of (0 to 'count') events from the specified stream.</returns>
    Task EnumerateStreamChunks(TStreamId streamId, Action<IEnumerable<StreamEvent>> process, long chunkSize = 50);
    /// <summary>
    /// Enumerates all events , reading a limited number of events (specified by chunkSize)
    /// each time until all events are read.
    /// </summary>
    /// <param name="process">After each read, this method will be called to process retrieved events.</param>
    /// <param name="chunkSize">Maximum number of events to be read in each step</param>
    /// <returns>A collection of (0 to 'count') events from the specified stream.</returns>
    Task EnumerateChunks(Action<IEnumerable<StreamEvent>> process, long chunkSize = 50);
}