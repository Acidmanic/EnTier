using System;
using System.Collections.Generic;
using System.Text;

namespace Context
{
    public class InMemoryContext : IContext, IEnTierBuiltIn
    {

        private Dictionary<string,object> _data = new Dictionary<string, object>();
        public virtual void Apply()
        {
            //Applied! :D
        }

        public virtual void Dispose()
        {
            // Disposed ofcourse!
        }

        public IDataset<T> GetDataset<T>() where T : class
        {
            var key = typeof(T).GUID.ToString();

            DataListDataset<T> dataset;
            if (_data.ContainsKey(key))
            {
                dataset = new DataListDataset<T>();

                _data.Add(key, dataset);
            }
            else
            {
                dataset = (DataListDataset<T>)_data[key];
            }

            return dataset;
            
        }
    }
}
