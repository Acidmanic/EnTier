using System;
using System.Collections.Generic;
using System.Text;

namespace Components
{
    abstract class ComponentProductionStrategyBase
    {

        private readonly List<IComponentProvider> _providers;


        public ComponentProductionStrategyBase()
        {
            _providers = new List<IComponentProvider>();

            SetUpProviders(_providers);
        }


        protected abstract void SetUpProviders(List<IComponentProvider> providers);


        public T Produce<T>(params object[] args)
        {
            for(int i = 0; i < _providers.Count; i++)
            {
                var p = _providers[i];

                try
                {
                    var ret = p.Provide<T>(args);

                    if (ret != null) return ret;
                }
                catch (Exception)
                {                }
            }

            return default;
        }

    }
}
