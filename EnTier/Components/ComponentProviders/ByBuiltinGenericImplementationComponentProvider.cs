using System;
using System.Collections.Generic;
using System.Text;
using Utility;
using System.Linq;

namespace Components
{
    class ByBuiltinGenericImplementationComponentProvider : IComponentProvider
    {
        public TInterface Provide<TInterface>(params object[] args)
        {
            var constructor = ReflectionService.Make()
                .FilterAlloweImplementers<IEnTierGeneric>()
                .FindConstructor<TInterface>(args);

            return constructor.Construct();
        }
    }
}
