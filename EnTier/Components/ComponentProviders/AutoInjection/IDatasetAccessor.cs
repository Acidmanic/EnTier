using EnTier.Context;
using System;
using System.Collections.Generic;
using System.Text;

namespace EnTier.Components
{
    public interface IDatasetAccessor
    {
        IDataset<T> Get<T>() where T : class;
    }
}
