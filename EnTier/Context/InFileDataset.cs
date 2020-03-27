using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Context
{
    
    [Serializable]
    public class InFileDataset<TEntity> : IDataset<TEntity>, IEnTierBuiltIn
        where TEntity : class
    {

        public long LastId { get; set; }

        private List<TEntity> _data = new List<TEntity>();

        public TEntity Add(TEntity item)
        {
            var id = Utility.Reflection.GetProperty<long>(item, "Id");

            LastId += 1;

            id.Value = LastId ;

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
