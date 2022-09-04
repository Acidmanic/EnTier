using System;

namespace EnTier.Prepopulation
{
    public interface IServiceResolver
    {
        object Resolve(Type abstractionType);
    }
}