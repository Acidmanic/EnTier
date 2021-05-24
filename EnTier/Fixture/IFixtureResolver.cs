using System;

namespace EnTier.Fixture
{
    public interface IFixtureResolver
    {
        T Resolve<T>();

        object Resolve(Type type);
    }
}