using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Acidmanic.Utilities.Reflection.Extensions;
using Acidmanic.Utilities.Reflection.ObjectTree;
using Acidmanic.Utilities.Reflection.TypeCenter;
using EnTier.Repositories;
using EnTier.Repositories.Attributes;
using EnTier.Utility;
using Microsoft.Extensions.FileProviders;

namespace EnTier.DataAccess.InMemory
{
    public class InMemoryRepository<TStorage, TId> : CrudRepositoryBase<TStorage, TId>
        where TStorage : class, new()
    {
        private readonly List<TStorage> _data = new List<TStorage>();
        private readonly  IdGenerator _idGenerator = new IdGenerator();
        private readonly AccessNode _idLeaf =  IdHelper.GetIdNode<TStorage, TId>();
        
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
            if (_idLeaf != null)
            {
                var id = (TId) _idLeaf.Evaluator.Read(value) ;

                if (id != null)
                {
                    if (Remove(id))
                    {
                        return this.Add(value);
                    }
                }
            }
            
            return default;
        }

        protected override TStorage Insert(TStorage value)
        {
            _data.Add(value);

            if (_idLeaf != null)
            {
                var id = _idGenerator.New<TId>();
                
                _idLeaf.Evaluator.Write(value,id);
            }
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
                    this.Remove(id);
                }
            }

            return this.Add(value);
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