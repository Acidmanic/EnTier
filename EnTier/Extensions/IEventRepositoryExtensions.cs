using System.Collections.Generic;
using System.Threading.Tasks;
using EnTier.Repositories;

namespace EnTier.Extensions;

public static class EventRepositoryExtensions
{
    /// <summary>
    /// Reads all events belonging to all streams.
    /// </summary>
    /// <returns>A collection of all events from all streams grouped by streamIds.</returns>
    public static async Task<Dictionary<TStreamId, List<TEvent>>> ReadStreamsGrouped<TEvent, TEventId, TStreamId>
        (this IEventStreamRepository<TEvent, TEventId, TStreamId> repository)
    {
        var groupedStreams = new Dictionary<TStreamId, List<TEvent>>();

        await repository.EnumerateChunks(chunk =>
        {
            foreach (var streamEvent in chunk)
            {
                if (!groupedStreams.ContainsKey(streamEvent.StreamId))
                {
                    groupedStreams.Add(streamEvent.StreamId, new List<TEvent>());
                }

                groupedStreams[streamEvent.StreamId].Add(streamEvent.Event);
            }
        });

        return groupedStreams;
    }
}