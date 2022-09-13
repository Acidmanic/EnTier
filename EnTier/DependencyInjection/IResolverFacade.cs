using System;

namespace EnTier.DependencyInjection
{
    public interface IResolverFacade
    {


        object Resolve(Type type);
    }
}