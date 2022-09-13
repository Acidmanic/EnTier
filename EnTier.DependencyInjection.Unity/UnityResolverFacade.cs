using System;
using Unity;

namespace EnTier.DependencyInjection.Unity
{
    public class UnityResolverFacade:IResolverFacade
    {

        private readonly IUnityContainer _container;

        public UnityResolverFacade(IUnityContainer container)
        {
            _container = container;
        }

        public object Resolve(Type type)
        {
            return _container.Resolve(type);
        }
    }
}