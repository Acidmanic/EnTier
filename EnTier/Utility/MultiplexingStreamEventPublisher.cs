using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EnTier.EventSourcing;

namespace EnTier.Utility;
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
        Add(() => (e,i,s) => publisherFactory().Publish(e,i,s));
    }
    
    public void Add(IStreamEventPublisherAdapter adapter)
    {
        Add(() => adapter.Publish);
    }
}