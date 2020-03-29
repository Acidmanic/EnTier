using System;
using System.Collections.Generic;
using System.Text;

namespace EnTier.Components
{
    class MapperProviderStrategy : ComponentProductionStrategyBase
    {
        protected override void SetUpProviders(List<IComponentProvider> providers)
        {
            providers.Add(new ByInjectionProvider());

            providers.Add(new SimpleMapperProvider());
        }
    }
}
