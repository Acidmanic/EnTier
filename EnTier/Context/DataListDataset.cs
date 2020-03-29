using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EnTier.Context
{
    public class DataListDataset<T> : IDataset<T>, IEnTierBuiltIn
    where T:class
    {

        private List<T> _datalist = new List<T>();

        public T Add(T item)
        {
            _datalist.Add(item);

            return item;
        }

        public IQueryable<T> AsQueryable()
        {
            return _datalist.AsQueryable();
        }

        public T Remove(T item)
        {
            _datalist.Remove(item);

            return item;
        }
    }
}
