using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EnTier.EventSourcing;
using Microsoft.Extensions.Logging;

namespace EnTier.Utility.MultiplexingStreamEventPublisher
{
    /// <summary>
    /// This class helps multiplexing publication of events from a single source. Since this implements the
    /// <code>IStreamPublisherAdapter</code> interface, it can be registered in Di and be automatically detected by
    /// UnitOfWork. The <code>MultiplexingStreamEventPublisher</code> itself dose not store it's state. So you need to
    /// register it as Singleton.   
    /// </summary>
    public class MultiplexingStreamEventPublisher : IStreamEventPublisherAdapter
    {
        private readonly List<Func<Action<object, object, object>>> _publisherFactories =
            new List<Func<Action<object, object, object>>>();

        private readonly object _publishLock = new object();

        private readonly EnTierEssence _essence;

        public MultiplexingStreamEventPublisher(EnTierEssence essence)
        {
            _essence = essence;
        }

        public void Publish(object @event, object eventId, object streamId)
        {
            lock (_publishLock)
            {
                foreach (var factory in _publisherFactories)
                {
                    var publisher = factory();

                    Task.Run(() => publisher(@event, eventId, streamId));
                }
            }
        }

        public void Add(Func<Action<object, object, object>> publisherFactory)
        {
            lock (_publishLock)
            {
                _publisherFactories.Add(publisherFactory);
            }
        }

        public void Add(Action<object, object, object> publisher)
        {
            Add(() => publisher);
        }

        public void Add(Func<IStreamEventPublisherAdapter> publisherFactory)
        {
            Add(() => (e, i, s) => publisherFactory().Publish(e, i, s));
        }

        public void Add(IStreamEventPublisherAdapter adapter)
        {
            Add(() => adapter.Publish);
        }

        public void Add<TPublisher>()
            where TPublisher : IStreamEventPublisherAdapter
        {
            Add(typeof(TPublisher));
        }

        public void Add(Type publisherType)
        {
            Action<object, object, object> Factory()
            {
                var publisher = _essence.ResolveOrDefault(publisherType, () => null);

                if (publisher is IStreamEventPublisherAdapter adapter)
                {
                    return adapter.Publish;
                }

                return (o, o1, arg3) =>
                {
                    _essence.Logger.LogError(
                        "MultiplexingStreamEventPublisher was not able to instantiate " +
                        "{TypeName}. Please make sure this class is registered in your di", publisherType.FullName);
                };
            }

            Add(Factory);
        }
    }
}