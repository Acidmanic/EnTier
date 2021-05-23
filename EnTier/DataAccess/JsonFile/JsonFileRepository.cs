using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using EnTier.Repositories;

namespace EnTier.DataAccess.JsonFile
{
    public class JsonFileRepository<TStorage, TId> : ICrudRepository<TStorage, TId>
        where TStorage : class, new()
    {
        private readonly Dictionary<TId, TStorage> _index = new Dictionary<TId, TStorage>();
        private readonly List<TStorage> _data;
        private readonly IdGenerator _idGenerator = new IdGenerator();


        public JsonFileRepository(List<TStorage> data)
        {
            foreach (var storage in data)
            {
                Import(storage);
            }

            _data = data;
        }
        
        public IEnumerable<TStorage> All()
        {
            return _data;
        }

        private void Import(TStorage value)
        {
            var id = Utility.Reflection.GetPropertyReader<TStorage, TId>("Id")(value);

            _index.Add(id, value);

            _idGenerator.Taken(id);
        }

        public TStorage Add(TStorage value)
        {

            var id = (TId) _idGenerator.SetId(value);
            
            _index.Add(id, value);

            _idGenerator.Taken(id);
            
            _data.Add(value);

            return value;
        }

        public TStorage GetById(TId id)
        {
            if (_index.ContainsKey(id))
            {
                return _index[id];
            }

            return default;
        }

        public IEnumerable<TStorage> Find(Expression<Func<TStorage, bool>> predicate)
        {
            var found = new List<TStorage>();

            var isIncluded = predicate.Compile();

            foreach (var storage in _data)
            {
                if (isIncluded(storage))
                {
                    found.Add(storage);
                }
            }

            return found;
        }

        public bool Remove(TStorage value)
        {
            var id = Utility.Reflection.GetPropertyReader<TStorage, TId>("Id")(value);

            return Remove(id);
        }

        public bool Remove(TId id)
        {
            if (_index.ContainsKey(id))
            {
                var item = _index[id];

                _data.Remove(item);
                
                _idGenerator.Free(id);

                _index.Remove(id);

                return true;
            }
            return false;
        }
    }
}