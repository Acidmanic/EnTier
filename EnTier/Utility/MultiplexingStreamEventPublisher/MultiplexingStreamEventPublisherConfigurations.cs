using System;
using EnTier.EventSourcing;

namespace EnTier.Utility.MultiplexingStreamEventPublisher
{
    internal class MultiplexingStreamEventPublisherConfigurations : IMultiplexingStreamEventPublisherConfigurations
    {
        private readonly MultiplexingStreamEventPublisher _publisher;

        public MultiplexingStreamEventPublisherConfigurations(MultiplexingStreamEventPublisher publisher)
        {
            _publisher = publisher;
        }

        public IMultiplexingStreamEventPublisherConfigurations Add(
            Func<Action<object, object, object>> publisherFactory)
        {
            _publisher.Add(publisherFactory);

            return this;
        }

        public IMultiplexingStreamEventPublisherConfigurations Add(Action<object, object, object> publisher)
        {
            _publisher.Add(publisher);

            return this;
        }

        public IMultiplexingStreamEventPublisherConfigurations Add(Func<IStreamEventPublisherAdapter> publisherFactory)
        {
            _publisher.Add(publisherFactory);

            return this;
        }

        public IMultiplexingStreamEventPublisherConfigurations Add(IStreamEventPublisherAdapter adapter)
        {
            _publisher.Add(adapter);

            return this;
        }

        public IMultiplexingStreamEventPublisherConfigurations Add<TPublisher>()
            where TPublisher : IStreamEventPublisherAdapter
        {
            _publisher.Add<TPublisher>();

            return this;
        }

        public IMultiplexingStreamEventPublisherConfigurations Add(Type type)
        {
            _publisher.Add(type);

            return this;
        }

        internal class
            NullMultiplexingStreamEventPublisherConfigurations : IMultiplexingStreamEventPublisherConfigurations
        {
            public IMultiplexingStreamEventPublisherConfigurations Add(
                Func<Action<object, object, object>> publisherFactory)
            {
                return this;
            }

            public IMultiplexingStreamEventPublisherConfigurations Add(Action<object, object, object> publisher)
            {
                return this;
            }

            public IMultiplexingStreamEventPublisherConfigurations Add(
                Func<IStreamEventPublisherAdapter> publisherFactory)
            {
                return this;
            }

            public IMultiplexingStreamEventPublisherConfigurations Add(IStreamEventPublisherAdapter adapter)
            {
                return this;
            }

            public IMultiplexingStreamEventPublisherConfigurations Add<TPublisher>()
                where TPublisher : IStreamEventPublisherAdapter
            {
                return this;
            }

            public IMultiplexingStreamEventPublisherConfigurations Add(Type type)
            {
                return this;
            }
        }
    }
}