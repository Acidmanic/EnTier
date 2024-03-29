using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Acidmanic.Utilities.Filtering;
using Acidmanic.Utilities.Filtering.Models;
using Acidmanic.Utilities.Reflection;
using Acidmanic.Utilities.Reflection.Extensions;
using Acidmanic.Utilities.Reflection.ObjectTree;
using Acidmanic.Utilities.Reflection.TypeCenter;
using EnTier.Models;
using EnTier.Repositories;
using EnTier.Repositories.Attributes;
using EnTier.Utility;

namespace EnTier.DataAccess.JsonFile
{
    public class JsonFileRepository<TStorage, TId> : CrudRepositoryForwardAsyncBase<TStorage, TId>
        where TStorage : class, new()
    {
        private readonly Dictionary<TId, TStorage> _index = new Dictionary<TId, TStorage>();
        private readonly List<TStorage> _data;
        private readonly IdGenerator<TId> _idGenerator = new IdGenerator<TId>();
        private readonly AccessNode _idLeaf = TypeIdentity.FindIdentityLeaf<TStorage, TId>();

        private readonly List<MarkedFilterResult<TStorage, TId>> _filterResults;
        private readonly List<MarkedSearchIndex<TStorage, TId>> _searchIndex;

        public JsonFileRepository(List<TStorage> data, List<MarkedFilterResult<TStorage, TId>> filterResults,
            List<MarkedSearchIndex<TStorage, TId>> searchIndex)
        {
            foreach (var storage in data)
            {
                Import(storage);
            }

            _data = data;
            _filterResults = filterResults;
            _searchIndex = searchIndex;
        }

        public override IEnumerable<TStorage> All(bool readFullTree = false)
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

                _index.Add(id, value);
            }

            _data.Add(value);

            return value;
        }

        public override TStorage Set(TStorage value)
        {
            if (_idLeaf != null)
            {
                var id = (TId)_idLeaf.Evaluator.Read(value);

                if (id != null)
                {
                    Remove(id);
                }
            }

            return this.Add(value);
        }

        public override TStorage GetById(TId id, bool readFullTree = false)
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

        public override Task RemoveExpiredFilterResultsAsync()
        {
            return ObjectListRepositoryFilteringHelper.RemoveExpiredFilterResultsAsync(_filterResults);
        }

        public override Task<FilterResponse> PerformFilterIfNeededAsync(
            FilterQuery filterQuery,
            string searchId = null,
            string[] searchTerms = null,
            OrderTerm[] orderTerms = null,
            bool readFullTree = false)
        {
            return ObjectListRepositoryFilteringHelper
                .PerformFilterIfNeededAsync(
                    _filterResults,
                    _searchIndex,
                    _idLeaf, _data, filterQuery, searchTerms, orderTerms, searchId);
        }

        public override Task<IEnumerable<TStorage>> ReadChunkAsync(int offset, int size, string searchId,
            bool readFullTree = false)
        {
            return ObjectListRepositoryFilteringHelper
                .ReadChunkAsync(_filterResults, _idLeaf, _data, offset, size, searchId);
        }

        public override Task<SearchIndex<TId>> IndexAsync(TId id, string indexCorpus)
        {
            long indexId = _searchIndex.Count > 0 ? _searchIndex.Select(i => i.Id).Max() + 1 : 1;

            var existing = _searchIndex.FirstOrDefault(i => i.ResultId.Equals(id));

            if (existing != null)
            {
                _searchIndex.Remove(existing);
            }

            var indexRecord = new SearchIndex<TId>
            {
                IndexCorpus = indexCorpus,
                ResultId = id,
                Id = indexId
            };

            _searchIndex.Add(indexRecord.AsMarked<TStorage, TId>());

            return Task.FromResult(indexRecord);
        }
    }
}