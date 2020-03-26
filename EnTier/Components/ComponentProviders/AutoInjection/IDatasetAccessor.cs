using Context;
using System;
using System.Collections.Generic;
using System.Text;

namespace Components
{
    public interface IDatasetAccessor
    {
        IDataset<T> Get<T>() where T : class;
    }
}
