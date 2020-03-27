using Mapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace Components
{
    class SimpleMapperProvider : IComponentProvider
    {
        public T Provide<T>(params object[] args)
        {
            return (T)(object)new SimpleMapper();
        }
    }
}
