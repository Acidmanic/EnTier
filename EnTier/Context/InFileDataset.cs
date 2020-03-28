using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Context
{
    
    public class InFileDataset<TEntity> : IDataset<TEntity>, IEnTierBuiltIn
        where TEntity : class
    {

        private List<TEntity> _data;


        public InFileDataset(List<TEntity> data)
        {
            _data = data;
        }

        public TEntity Add(TEntity item)
        {
            var id = Utility.Reflection.GetProperty<long>(item, "Id");

            id.Value = _data.Count;

            _data.Add(item);

            return item;
        }

        public IQueryable<TEntity> AsQueryable()
        {
            return _data.AsQueryable<TEntity>();
        }

        public TEntity Remove(TEntity item)
        {
            _data.Remove(item);

            return item;
        }
    }
}
