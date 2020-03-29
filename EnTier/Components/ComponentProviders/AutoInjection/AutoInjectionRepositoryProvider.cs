using System;
using System.Collections.Generic;
using System.Text;

namespace EnTier.Components
{
    class AutoInjectionRepositoryProvider : AutoInjectionComponentProvider
    {


        public override TInterface Provide<TInterface>(params object[] args)
        {

            TInterface ret = default;

            var obj = new Object();

            lock (obj)
            {
                SingletonInjectionDatasetProvider.Make().Set(args[0]);
                ret = base.Provide<TInterface>();
                SingletonInjectionDatasetProvider.Make().Set(null);
            }

            return ret;
        }
    }
}
