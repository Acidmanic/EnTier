using EnTier.Context;
using System;
using System.Collections.Generic;
using System.Text;

namespace EnTier.Components
{
    class ContextProducingStrategy : ComponentProductionStrategyBase
    {
        protected override void SetUpProviders(List<IComponentProvider> providers)
        {
            providers.Add(new AppConfigurationContextProvider());

            providers.Add(new ByConventionComponentProvider());

            providers.Add(new SearchForEfDbContextComponentProvider());

            providers.Add(new NullContextProvider());
        }
    }
}
