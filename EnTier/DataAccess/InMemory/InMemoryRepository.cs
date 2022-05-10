using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using EnTier.Repositories;

namespace EnTier.DataAccess.InMemory
{
    public class InMemoryRepository<TStorage, TId> : CrudRepositoryBase<TStorage, TId>
        where TStorage : class, new()
    {
        private readonly List<TStorage> _data = new List<TStorage>();

        public override IEnumerable<TStorage> All()
        {
            return _data;
        }

        public override TStorage Add(TStorage value)
        {
            _data.Add(value);

            return value;
        }

        public override TStorage GetById(TId id)
        {
            var idReader = Utility.Reflection.GetPropertyReader<TStorage, TId>("Id");

            return _data.Find(s => id.Equals(idReader(s)));
        }

        public override IEnumerable<TStorage> Find(Expression<Func<TStorage, bool>> predicate)
        {
            var result = new List<TStorage>();

            var isDesired = predicate.Compile();

            foreach (var item in _data)
            {
                if (isDesired(item))
                {
                    result.Add(item);
                }
            }

            return result;
        }

        public override bool Remove(TStorage value)
        {
            var id = Utility.Reflection.GetPropertyReader<TStorage, TId>("Id").Invoke(value);

            return Remove(id);
        }

        public override bool Remove(TId id)
        {
            int index = 0;

            var idReader = Utility.Reflection.GetPropertyReader<TStorage, TId>("Id");

            bool removedAny = false;

            while (index < _data.Count)
            {
                var itemId = idReader.Invoke(_data[index]);

                if (id.Equals(itemId))
                {
                    _data.RemoveAt(index);
                    
                    removedAny = true;
                }
                else
                {
                    index++;
                }
            }
            return removedAny;
        }
    }
}