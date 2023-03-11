using System;
using EnTier.EventSourcing;

namespace EnTier.Utility.MultiplexingStreamEventPublisher
{
    public interface IMultiplexingStreamEventPublisherConfigurations
    {
        IMultiplexingStreamEventPublisherConfigurations Add(Func<Action<object, object, object>> publisherFactory);

        IMultiplexingStreamEventPublisherConfigurations Add(Action<object, object, object> publisher);

        IMultiplexingStreamEventPublisherConfigurations Add(Func<IStreamEventPublisherAdapter> publisherFactory);

        IMultiplexingStreamEventPublisherConfigurations Add(IStreamEventPublisherAdapter adapter);

        IMultiplexingStreamEventPublisherConfigurations Add<TPublisher>()
            where TPublisher : IStreamEventPublisherAdapter;

        IMultiplexingStreamEventPublisherConfigurations Add(Type type);
    }
}