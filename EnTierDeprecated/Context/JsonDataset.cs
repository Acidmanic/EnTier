using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EnTier.Context
{
    
    internal class JsonDataset<TEntity> : IDataset<TEntity>, IEnTierBuiltIn
        where TEntity : class
    {

        private List<TEntity> _data;
        private IIDGenerator _idGenerator;

        public JsonDataset(List<TEntity> data,IIDGenerator idGenerator)
        {
            _data = data;

            _idGenerator = idGenerator;
        }

        public TEntity Add(TEntity item)
        {
            var id = Utility.Reflection.GetProperty<long>(item, "Id");

            id.Value = _idGenerator.NewId<TEntity>();

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
