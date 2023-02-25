using System.Collections.Generic;

namespace EnTier.EventSourcing;

public interface IAggregate<TAggregateRoot,TEvent,TStreamId>:IProjection<TAggregateRoot,TEvent,TStreamId>
{
    
    public List<TEvent> Updates { get; }
    
    public List<TEvent> Events { get; }
    
}

public static class AggregateExtensions
{
    public static void Initialize<TEntity, TEvent, TStreamId>(this IAggregate<TEntity, TEvent, TStreamId> aggregate,
        TStreamId streamId)
    {
        aggregate.Initialize(streamId, new List<TEvent>());
    }
}