using System;
using System.Collections.Generic;
using Acidmanic.Utilities.Reflection;

namespace EnTier.EventSourcing
{
    /// <summary>
    /// This is a base class to ease implementation of projection classes. It pre implements common tasks of a projection.
    /// </summary>
    /// <typeparam name="TEntity">Tht type of the state object which is going to be projected with Projection class.</typeparam>
    /// <typeparam name="TEvent">The type of events that this Projection class would process.</typeparam>
    /// <typeparam name="TStreamId">The type of stream identifier.</typeparam>
    public abstract class ProjectionBase<TEntity, TEvent, TStreamId> : IProjection<TEntity, TEvent, TStreamId>
    {
        public List<TEvent> Events { get; } = new List<TEvent>();

        public TStreamId StreamId => _streamId;

        public TEntity CurrentState { get; }


        private TStreamId _streamId = default;

        public ProjectionBase()
        {
            // ReSharper disable once VirtualMemberCallInConstructor
            CurrentState = InstantiateStateObject();
        }

        public void Initialize(TStreamId streamId, IEnumerable<TEvent> events)
        {
            _streamId = streamId;

            foreach (var orderEvent in events)
            {
                Apply(orderEvent);
            }
        }

        public void Apply(TEvent @event)
        {
            ManipulateState(@event);

            Events.Add(@event);
        }

        protected abstract void ManipulateState(TEvent @event);

        protected TEntity InstantiateStateObject()
        {
            var type = typeof(TEntity);

            var value = new ObjectInstantiator().BlindInstantiate(type);

            if (value != null)
            {
                try
                {
                    var instance = (TEntity)value;

                    return instance;
                }
                catch (Exception _){/**/}
            }

            return default;
        }
    }
}