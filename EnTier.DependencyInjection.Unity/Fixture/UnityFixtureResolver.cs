using System;
using EnTier.Fixture;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Unity;

namespace EnTier.DependencyInjection.Unity.Fixture
{
    internal class UnityFixtureResolver:IFixtureResolver
    {
        private readonly IUnityContainer _container;

        public UnityFixtureResolver(IApplicationBuilder applicationBuilder)
        {
            _container = applicationBuilder.ApplicationServices.GetService<IUnityContainer>();
        }

        public T Resolve<T>()
        {
            return _container.Resolve<T>();
        }

        public object Resolve(Type type)
        {
            return _container.Resolve(type);
        }
    }
}