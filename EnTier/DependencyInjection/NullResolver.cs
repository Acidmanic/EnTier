using System;

namespace EnTier.DependencyInjection
{
    internal class NullResolver:ResolverAdapter
    {
        public NullResolver() : base(t => null) { }
    }
}