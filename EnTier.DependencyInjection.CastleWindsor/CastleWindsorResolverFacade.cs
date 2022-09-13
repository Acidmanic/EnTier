using System;
using Castle.Windsor;

namespace EnTier.DependencyInjection.CastleWindsor
{
    public class CastleWindsorResolverFacade:IResolverFacade
    {

        private readonly IWindsorContainer _container;

        public CastleWindsorResolverFacade(IWindsorContainer container)
        {
            _container = container;
        }

        public object Resolve(Type type)
        {
            return _container.Resolve(type);
        }
    }
}