using System;
using System.Collections.Generic;
using System.Text;

namespace Components
{
    class RepositoryProducingStrategy : ComponentProductionStrategyBase
    {
        protected override void SetUpProviders(List<IComponentProvider> providers)
        {
            providers.Add(new AutoInjectionRepositoryProvider());
            providers.Add(new ByConventionComponentProvider());
            providers.Add(new ByBuiltinGenericImplementationComponentProvider());
            //TODO: manage NullObject pattern 
        }
    }
}
