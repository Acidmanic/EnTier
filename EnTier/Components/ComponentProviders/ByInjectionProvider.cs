using System;
using System.Collections.Generic;
using System.Text;
using Utility;

namespace Components
{
    class ByInjectionProvider : IComponentProvider
    {
        public T Provide<T>(params object[] args)
        {
            return EnTierApplication.Resolver.Resolve<T>();
        }
    }
}
