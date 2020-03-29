using System;
using System.Collections.Generic;
using System.Text;

namespace EnTier.Components
{
    class ServiceProducingStrategy : ComponentProductionStrategyBase
    {
        protected override void SetUpProviders(List<IComponentProvider> providers)
        {
            providers.Add(new AutoInjectionComponentProvider());
            providers.Add(new ByConventionComponentProvider());
            providers.Add(new ByBuiltinGenericImplementationComponentProvider());
            //TODO: manage NullObject pattern 

        }
    }
}
