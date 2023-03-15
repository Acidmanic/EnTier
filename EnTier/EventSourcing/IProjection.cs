using System.Collections.Generic;

namespace EnTier.EventSourcing
{
    /// <summary>
    /// Implementing this interface would create a Projection class which represents a specific aspect/state
    /// for a stream of events. 
    /// </summary>
    /// <typeparam name="TEntity">The type of the state object which is going to be projected with Projection class.</typeparam>
    /// <typeparam name="TEvent">The type of events that this Projection class would process.</typeparam>
    /// <typeparam name="TStreamId">The type of stream identifier.</typeparam>
    public interface IProjection<TEntity, TEvent, TStreamId>
    {
        void Initialize(TStreamId streamId, IEnumerable<TEvent> events);

        public TEntity CurrentState { get; }

        public void Apply(TEvent @event);

        public TStreamId StreamId { get; }

        /// <summary>
        /// This property holds all events in history of the aggregate from beginning.
        /// </summary>
        public List<TEvent> Events { get; }
    }

    public static class ProjectionExtensions
    {
        /// <summary>
        /// This methods calls projection class's Initialize method with no events, so the projection object would be
        /// in a reset state and ready for further events to be added later.
        /// </summary>
        /// <param name="projection">Projection object</param>
        /// <param name="streamId">
        /// The unique identifier of the stream that this projection is supposed to
        /// represent it's state.
        /// </param>
        /// <typeparam name="TEntity">The type of the state object which is going to be projected with Projection class.</typeparam>
        /// <typeparam name="TEvent">The type of events that this Projection class would process.</typeparam>
        /// <typeparam name="TStreamId">The type of stream identifier.</typeparam>
        public static void Initialize<TEntity, TEvent, TStreamId>(
            this IProjection<TEntity, TEvent, TStreamId> projection,
            TStreamId streamId)
        {
            projection.Initialize(streamId, new List<TEvent>());
        }
    }
}