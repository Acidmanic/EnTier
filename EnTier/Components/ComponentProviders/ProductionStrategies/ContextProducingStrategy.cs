using Context;
using System;
using System.Collections.Generic;
using System.Text;

namespace Components.ComponentProviders.ProductionStrategies
{
    class ContextProducingStrategy : ComponentProductionStrategyBase
    {
        protected override void SetUpProviders(List<IComponentProvider> providers)
        {
            providers.Add(new ByConventionComponentProvider());

            providers.Add(new SearchForEfDbContextComponentProvider());
        }
    }
}
