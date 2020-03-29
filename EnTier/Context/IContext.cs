


using System;

namespace EnTier.Context
{



    public interface IContext:IDisposable
    {

        void Apply();

        IDataset<T> GetDataset<T>() where T:class;
        
    }
}