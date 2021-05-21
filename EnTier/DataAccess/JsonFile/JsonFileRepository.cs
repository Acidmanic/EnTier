using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using EnTier.Repositories;

namespace EnTier.DataAccess.JsonFile
{
    public class JsonFileRepository<TStorage, TId> : ICrudRepository<TStorage, TId>
        where TStorage : class, new()
    {
        private Dictionary<TId, TStorage> _data = new Dictionary<TId, TStorage>();
        private readonly IDGenerator<TId> _idGenerator = new IDGenerator<TId>();


        public JsonFileRepository(IEnumerable<TStorage> data)
        {
            foreach (var storage in data)
            {
                Import(storage);
            }
        }
        
        public IEnumerable<TStorage> All()
        {
            return _data.Values;
        }

        private void Import(TStorage value)
        {
            var id = Utility.Reflection.GetPropertyReader<TStorage, TId>("Id")(value);

            _data.Add(id, value);

            _idGenerator.Taken(id);
        }

        public TStorage Add(TStorage value)
        {
            var id = _idGenerator.New();

            Utility.Reflection.GetPropertyWriter<TStorage, TId>("Id").Invoke(value, id);

            _data.Add(id, value);

            _idGenerator.Taken(id);

            return value;
        }

        public TStorage GetById(TId id)
        {
            if (_data.ContainsKey(id))
            {
                return _data[id];
            }

            return default;
        }

        public IEnumerable<TStorage> Find(Expression<Func<TStorage, bool>> predicate)
        {
            var found = new List<TStorage>();

            var isIncluded = predicate.Compile();

            foreach (var storage in _data.Values)
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
            if (_data.ContainsKey(id))
            {
                _idGenerator.Free(id);

                _data.Remove(id);

                return true;
            }
            return false;
        }
    }
}