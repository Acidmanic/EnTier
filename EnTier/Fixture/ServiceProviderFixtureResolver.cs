using System;
using Microsoft.Extensions.DependencyInjection;

namespace EnTier.Fixture
{
    public class ServiceProviderFixtureResolver : IFixtureResolver
    {
        private readonly IServiceProvider _serviceCollection;

        public ServiceProviderFixtureResolver(IServiceProvider serviceCollection)
        {
            _serviceCollection = serviceCollection;
        }

        public T Resolve<T>()
        {
            try
            {
                return _serviceCollection.GetService<T>();
            }
            catch (Exception _)
            {
                return default;
            }
        }

        public object Resolve(Type type)
        {
            try
            {
                return _serviceCollection.GetService(type);
            }
            catch (Exception _)
            {
                return null;
            }
        }
    }
}