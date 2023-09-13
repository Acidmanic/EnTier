using System;

namespace EnTier.Prepopulation.Contracts
{
    public interface IServiceResolver
    {
        object Resolve(Type abstractionType);
    }
}