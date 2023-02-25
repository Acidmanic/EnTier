using System.Collections.Generic;

namespace EnTier.EventSourcing;

public sealed class NullAggregate<TAggregateRoot, TEvent, TStreamId> : IAggregate<TAggregateRoot, TEvent, TStreamId>
{
    public void Initialize(TStreamId streamId, IEnumerable<TEvent> events)
    {
    }

    public TAggregateRoot CurrentState { get; } = default;

    public void Apply(TEvent @event)
    {
    }

    public TStreamId StreamId { get; } = default;
    public List<TEvent> Updates { get; } = new List<TEvent>();
    public List<TEvent> Events { get; } = new List<TEvent>();
}