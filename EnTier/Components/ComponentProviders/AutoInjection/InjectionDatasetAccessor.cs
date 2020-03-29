using EnTier.Context;
using System;
using System.Collections.Generic;
using System.Text;

namespace EnTier.Components
{
    internal class InjectionDatasetAccessor : IDatasetAccessor
    {
        public IDataset<T> Get<T>() where T : class
        {
            return SingletonInjectionDatasetProvider.Make().Get<T>();
        }
    }
}
