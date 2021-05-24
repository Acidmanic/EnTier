using System;
using Castle.Windsor;
using EnTier.Fixture;
using Microsoft.Extensions.DependencyInjection;

namespace EnTier.DependencyInjection.CastleWindsor
{
    internal class CastleWindsorFixtureResolver : IFixtureResolver
    {
        private readonly IWindsorContainer _container;

        public CastleWindsorFixtureResolver(IServiceProvider serviceCollection)
        {
            _container = serviceCollection.GetService<IWindsorContainer>();
        }


        public CastleWindsorFixtureResolver(IWindsorContainer container)
        {
            _container = container;
        }

        public T Resolve<T>()
        {
            try
            {
                return _container.Resolve<T>();
            }
            catch (Exception e)
            {
                return default;
            }
        }

        public object Resolve(Type type)
        {
            try
            {
                return _container.Resolve(type);
            }
            catch (Exception e)
            {
                return null;
            }
        }
    }
}