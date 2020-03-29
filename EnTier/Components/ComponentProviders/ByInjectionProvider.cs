using System;
using System.Collections.Generic;
using System.Text;
using EnTier.Utility;

namespace EnTier.Components
{
    class ByInjectionProvider : IComponentProvider
    {
        public T Provide<T>(params object[] args)
        {
            return EnTierApplication.Resolver.Resolve<T>();
        }
    }
}
