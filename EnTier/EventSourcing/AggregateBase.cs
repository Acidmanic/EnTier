using System;
using System.Collections.Generic;
using Acidmanic.Utilities.Reflection;

namespace EnTier.EventSourcing;

/// <summary>
/// This is a base class to ease implementation of aggregate classes. It pre implements common tasks of an aggregate.
/// </summary>
/// <typeparam name="TAggregateRoot"></typeparam>
/// <typeparam name="TEvent"></typeparam>
/// <typeparam name="TStreamId"></typeparam>
public abstract class AggregateBase<TAggregateRoot, TEvent, TStreamId> :
    ProjectionBase<TAggregateRoot, TEvent, TStreamId>,
    IAggregate<TAggregateRoot, TEvent, TStreamId>
{
    public List<TEvent> Updates { get; } = new List<TEvent>();

    protected abstract override void ManipulateState(TEvent @event);

    /// <summary>
    /// This method must be called whenever an event is produced due to performing of the business logics.
    /// </summary>
    /// <param name="event">Produced event</param>
    protected void ActionTaken(TEvent @event)
    {
        ManipulateState(@event);

        Updates.Add(@event);
    }
}