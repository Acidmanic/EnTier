using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Acidmanic.Utilities.Reflection;
using Acidmanic.Utilities.Reflection.Extensions;
using Acidmanic.Utilities.Reflection.ObjectTree;
using Acidmanic.Utilities.Reflection.TypeCenter;
using EnTier.Repositories;
using EnTier.Repositories.Attributes;
using EnTier.Utility;

namespace EnTier.DataAccess.InMemory
{
    public class InMemoryRepository<TStorage, TId> : CrudRepositoryForwardAsyncBase<TStorage, TId>
        where TStorage : class, new()
    {
        private readonly List<TStorage> _data = new List<TStorage>();
        private readonly IdGenerator<TId> _idGenerator = new IdGenerator<TId>();
        private readonly AccessNode _idLeaf = TypeIdentity.FindIdentityLeaf<TStorage, TId>();

        public override IEnumerable<TStorage> All()
        {
            return _data;
        }

        [KeepAllProperties()]
        public override TStorage Add(TStorage value)
        {
            return base.Add(value);
        }

        public override TStorage Update(TStorage value)
        {
            if (value != null)
            {
                if (_idLeaf != null)
                {
                    var id = (TId)_idLeaf.Evaluator.Read(value);

                    if (id != null)
                    {
                        var data = GetById(id);

                        if (data != null)
                        {
                            value.CopyInto(data);

                            return data;
                        }
                    }
                }
            }

            return default;
        }

        protected override TStorage Insert(TStorage value)
        {
            if (_idLeaf != null && _idLeaf.IsAutoValued)
            {
                var id = _idGenerator.New();

                _idGenerator.Taken(id);

                _idLeaf.Evaluator.Write(value, id);
            }

            _data.Add(value);

            return value;
        }

        public override TStorage Set(TStorage value)
        {
            if (_idLeaf != null)
            {
                var id = (TId)_idLeaf.Evaluator.Read(value);

                var found = _data.Find(s => _idLeaf.Evaluator.Read(s).AreEquivalentsWith(id));

                if (found != default)
                {
                    return Update(value);
                }

                return Add(value);
            }

            return default;
        }

        public override TStorage GetById(TId id)
        {
            if (_idLeaf != null)
            {
                return _data.Find(s => _idLeaf.Evaluator.Read(s).AreEquivalentsWith(id));
            }

            return default;
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
            var id = TypeCenter.GetPropertyReader<TStorage, TId>("Id").Invoke(value);

            return Remove(id);
        }

        public override bool Remove(TId id)
        {
            int index = 0;

            var idReader = TypeCenter.GetPropertyReader<TStorage, TId>("Id");

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