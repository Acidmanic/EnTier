using System;
using System.Collections.Generic;
using System.Text;
using EnTier.Utility;

namespace EnTier.Components
{
    class ByConventionComponentProvider : IComponentProvider
    {
        public TInterface Provide<TInterface>(params object[] args)
        {
            var constructor = ReflectionService.Make()
                .FilterRemoveImplementers<IEnTierGeneric>()
                .FilterRemoveImplementers<IEnTierBuiltIn>()
                .FindConstructor<TInterface>(args);

            return constructor.Construct();
        }
    }
}
