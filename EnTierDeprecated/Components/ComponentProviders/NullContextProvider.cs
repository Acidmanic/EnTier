using EnTier.Context;
using System;
using System.Collections.Generic;
using System.Text;

namespace EnTier.Components
{
    class NullContextProvider : IComponentProvider
    {
        public T Provide<T>(params object[] args)
        {
            return (T)(object)new NullContext();
        }
    }
}
