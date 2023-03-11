using System.Collections.Generic;

namespace EnTier.EventSourcing
{

    /// <summary>
    /// Implementing this interface would create an aggregate class which contains domain logic related to a specific
    /// entity (The AggregateRoot). An aggregate, when implemented, would perform business behavior and produces events
    /// whenever an action is taken. These events would be stored in <code>Updates</code> property and can be appended to
    /// an event stream. Aggregate classes would be instantiated per stream. The <code>StreamId</code> property of
    /// an aggregate, would identify corresponding event-stream for this aggregate. 
    /// </summary>
    /// <typeparam name="TAggregateRoot">
    /// The type of the state object (Aggregate root) which is going to be manipulated with this aggregate class.
    /// </typeparam>
    /// <typeparam name="TEvent">The type of events that this aggregate class would process.</typeparam>
    /// <typeparam name="TStreamId">The type of stream identifier.</typeparam>
    public interface IAggregate<TAggregateRoot, TEvent, TStreamId> : IProjection<TAggregateRoot, TEvent, TStreamId>
    {
        /// <summary>
        /// This property would hold any event produced by aggregate class and can be stored (appended) to event stream.
        /// </summary>
        public List<TEvent> Updates { get; }
    }

    public static class AggregateExtensions
    {
        /// <summary>
        /// This methods calls aggregate class's Initialize method with no events, so the aggregate object would be
        /// in a reset state and ready to updated later.
        /// </summary>
        /// <param name="aggregate">Aggregate object</param>
        /// <param name="streamId">
        /// The unique identifier of the stream that this projection is supposed to
        /// represent it's state.
        /// </param>
        /// <typeparam name="TAggregateRoot">The Aggregate root's type.</typeparam>
        /// <typeparam name="TEvent">The type of events that this Projection class would process.</typeparam>
        /// <typeparam name="TStreamId">The type of stream identifier.</typeparam>
        public static void Initialize<TAggregateRoot, TEvent, TStreamId>(
            this IAggregate<TAggregateRoot, TEvent, TStreamId> aggregate,
            TStreamId streamId)
        {
            aggregate.Initialize(streamId, new List<TEvent>());
        }

        /// <summary>
        /// Checks if the aggregate is just instantiated and is absolutely in the t=0 moment of it's history. 
        /// </summary>
        /// <param name="aggregate">The Aggregate object</param>
        /// <typeparam name="TAggregateRoot">The Aggregate root's type.</typeparam>
        /// <typeparam name="TEvent">The type of events that this Projection class would process.</typeparam>
        /// <typeparam name="TStreamId">The type of stream identifier.</typeparam>
        public static bool IsPristine<TAggregateRoot, TEvent, TStreamId>(
            this IAggregate<TAggregateRoot, TEvent, TStreamId> aggregate)
        {
            return aggregate.Events.Count == 0 && aggregate.Updates.Count == 0;
        }
    }
}