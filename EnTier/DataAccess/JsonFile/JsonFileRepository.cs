using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Acidmanic.Utilities.Reflection.TypeCenter;
using EnTier.Repositories;
using EnTier.Repositories.Attributes;

namespace EnTier.DataAccess.JsonFile
{
    public class JsonFileRepository<TStorage, TId> : CrudRepositoryBase<TStorage, TId>
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

        public override IEnumerable<TStorage> All()
        {
            return _data;
        }

        private void Import(TStorage value)
        {
            var id = TypeCenter.GetPropertyReader<TStorage, TId>("Id")(value);

            _index.Add(id, value);

            _idGenerator.Taken(id);
        }
        
        [KeepAllProperties()]
        public override TStorage Add(TStorage value)
        {
            return base.Add(value);
        } 

        protected override TStorage Insert(TStorage value)
        {
            var id = (TId) _idGenerator.SetId(value);

            _index.Add(id, value);

            _idGenerator.Taken(id);

            _data.Add(value);

            return value;
        }

        public override TStorage GetById(TId id)
        {
            if (_index.ContainsKey(id))
            {
                return _index[id];
            }

            return default;
        }

        public override IEnumerable<TStorage> Find(Expression<Func<TStorage, bool>> predicate)
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

        public override bool Remove(TStorage value)
        {
            var id = TypeCenter.GetPropertyReader<TStorage, TId>("Id")(value);

            return Remove(id);
        }

        public override bool Remove(TId id)
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