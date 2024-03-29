using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Acidmanic.Utilities.DataTypes;
using Acidmanic.Utilities.Filtering;
using Acidmanic.Utilities.Filtering.Extensions;
using Acidmanic.Utilities.Filtering.Models;
using Acidmanic.Utilities.Reflection;
using Acidmanic.Utilities.Reflection.Extensions;
using EnTier.DataAccess.EntityFramework.Extensions;
using EnTier.DataAccess.EntityFramework.FullTreeHandling;
using EnTier.DataAccess.JsonFile;
using EnTier.Models;
using EnTier.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EnTier.DataAccess.EntityFramework
{
    public class EntityFrameWorkCrudRepository<TStorage, TId> : CrudRepositoryForwardAsyncBase<TStorage, TId>
        where TStorage : class, new()
    {
        protected DbSet<TStorage> DbSet { get; }
        protected DbSet<MarkedFilterResult<TStorage, TId>> FilterResults { get; }
        protected DbSet<MarkedSearchIndex<TStorage, TId>> SearchIndex { get; }

        protected IFullTreeMarker<TStorage> FullTreeMarker { get; }

        internal EntityFrameWorkCrudRepository(DbSet<TStorage> dbSet,
            DbSet<MarkedFilterResult<TStorage, TId>> filterResults,
            DbSet<MarkedSearchIndex<TStorage, TId>> searchIndex,
            [AllowNull] IFullTreeMarker<TStorage> fullTreeMarker)
        {
            DbSet = dbSet;
            FilterResults = filterResults;
            FullTreeMarker = fullTreeMarker ?? new NullFullTreeMarker<TStorage>();
            SearchIndex = searchIndex;
        }


        protected virtual IQueryable<TStorage> FullTree(DbSet<TStorage> dbSet)
        {
            if (FullTreeMarker is NullFullTreeMarker)
            {
                Logger.LogWarning("Reaching for full tree {TypeName} object can not be achieved. " +
                                  "You need to register an implementation of IFullTreeMarker<{TypeName}> " +
                                  "on your Di system in order for this to work.",
                    typeof(TStorage).FullName, typeof(TStorage).FullName);
            }

            return FullTreeMarker.IncludeAsNeeded(dbSet);
        }


        protected IQueryable<TStorage> DbSetFullTreeAble(bool readFullTree)
        {
            return readFullTree ? FullTree(DbSet) : DbSet;
        }

        public override IEnumerable<TStorage> All(bool readFullTree = false)
        {
            return DbSetFullTreeAble(readFullTree).ToList();
        }

        protected override TStorage Insert(TStorage value)
        {
            var entry = DbSet.Add(value);

            if (entry.State == EntityState.Added)
            {
                return entry.Entity;
            }

            return default;
        }

        public override TStorage Update(TStorage value)
        {
            return DbSet.Update(value).Entity;
        }

        public override TStorage Set(TStorage value)
        {
            var idLeaf = TypeIdentity.FindIdentityLeaf<TStorage, TId>();

            if (idLeaf != null)
            {
                if (idLeaf.Evaluator.Read(value) is TId id)
                {
                    var found = DbSet.Find(id);

                    if (found != null)
                    {
                        value.CopyInto(found, idLeaf.GetFullName());

                        return DbSet.Update(found).Entity;
                    }
                }
            }

            return Insert(value);
        }

        public override TStorage GetById(TId id, bool readFullTree = false)
        {
            var idSelector = IdEqualsToExpression(id);

            var found = DbSetFullTreeAble(readFullTree).Where(idSelector).ToList().FirstOrDefault();

            return found;
        }

        public override IEnumerable<TStorage> Find(Expression<Func<TStorage, bool>> predicate)
        {
            var isIncluded = predicate.Compile();

            return DbSet.Where(isIncluded).ToList();
        }

        public override bool Remove(TStorage value)
        {
            return DbSet.Remove(value).State == EntityState.Deleted;
        }

        public override bool Remove(TId id)
        {
            var entity = GetById(id);

            if (entity != null)
            {
                return DbSet.Remove(entity).State == EntityState.Deleted;
            }

            return false;
        }

        public override Task RemoveExpiredFilterResultsAsync()
        {
            return Task.Run(() =>
            {
                var now = DateTime.Now.Ticks;

                var expired = FilterResults.Where(r => r.ExpirationTimeStamp <= now);

                FilterResults.RemoveRange(expired);
            });
        }

        public override Task<FilterResponse> PerformFilterIfNeededAsync(
            FilterQuery filterQuery,
            string searchId = null,
            string[] searchTerms = null,
            OrderTerm[] orderTerms = null,
            bool readFullTree = false)
        {
            searchId ??= Guid.NewGuid().ToString("N");

            orderTerms ??= new OrderTerm[] { };

            return Task.Run<FilterResponse>(() =>
            {
                var anyResults = FilterResults.Count(r => r.SearchId == searchId);

                if (anyResults < 1)
                {
                    var filterExpressions = filterQuery.ToExpression<TStorage>();

                    var queryable = DbSetFullTreeAble(readFullTree).AsQueryable();


                    foreach (var orderTerm in orderTerms)
                    {
                        var lambda = orderTerm.GetSelector<TStorage>();

                        if (orderTerm.Sort == OrderSort.Ascending)
                        {
                            queryable = queryable.OrderBy(lambda);
                        }
                        else if (orderTerm.Sort == OrderSort.Descending)
                        {
                            queryable = queryable.OrderByDescending(lambda);
                        }
                    }


                    foreach (var expression in filterExpressions)
                    {
                        queryable = queryable.Where(expression);
                    }

                    if (searchTerms != null && searchTerms.Length > 0)
                    {
                        var idSelector = PickIdExpression();

                        var indexedStorages = queryable.Join(
                            SearchIndex, idSelector, si => si.ResultId,
                            (st, si) => new { st, si });

                        var patterns = searchTerms.Select(t => $"%{t}%");

                        foreach (var pattern in patterns)
                        {
                            indexedStorages = indexedStorages.Where(combo =>
                                EF.Functions.Like(combo.si.IndexCorpus, pattern));
                        }

                        queryable = indexedStorages.Select(combo => combo.st);
                    }

                    var idLeaf = TypeIdentity.FindIdentityLeaf<TStorage>();

                    var expirationTime =
                        TimeStamp.Now.TotalMilliSeconds +
                        typeof(TStorage).GetFilterResultExpirationDurationMilliseconds();


                    var count = 0;
                    
                    foreach (var storage in queryable)
                    {
                        var filterResult = new FilterResult<TId>
                        {
                            SearchId = searchId,
                            ResultId = (TId)idLeaf.Evaluator.Read(storage),
                            ExpirationTimeStamp = expirationTime
                        };

                        FilterResults.Add(FilterResultsEfMarkingExtensions.AsMarked<TStorage, TId>(filterResult));

                        count++;
                    }
                  
                    var filterResponse = new FilterResponse
                    {
                        Count = count,
                        SearchId = searchId
                    };

                    return filterResponse;
                }

                return new FilterResponse
                {
                    Count = 0,
                    SearchId = searchId
                };
            });
        }

        private Expression<Func<TStorage, TId>> PickIdExpression()
        {
            var idLeaf = TypeIdentity.FindIdentityLeaf<TStorage>();
            // a
            ParameterExpression storageParameter = Expression.Parameter(typeof(TStorage), "a");
            //a.id
            MemberExpression storagesId = Expression.Property(storageParameter, idLeaf.Name);
            // a => a.id
            var lambda = Expression.Lambda<Func<TStorage, TId>>(storagesId, storageParameter);

            return lambda;
        }

        private Expression<Func<TStorage, bool>> IdEqualsToExpression(TId id)
        {
            var idLeaf = TypeIdentity.FindIdentityLeaf<TStorage>();
            // a=>
            ParameterExpression storageParameter = Expression.Parameter(typeof(TStorage), "a");
            //a=>a.id
            MemberExpression storagesId = Expression.Property(storageParameter, idLeaf.Name);

            Expression idValue = Expression.Constant(id);


            BinaryExpression selectExpression = Expression.Equal(storagesId, idValue);

            var lambda = Expression.Lambda<Func<TStorage, Boolean>>(selectExpression, storageParameter);

            return lambda;
        }


        public override Task<IEnumerable<TStorage>> ReadChunkAsync(int offset, int size, string searchId,
            bool readFullTree = false)
        {
            return Task.Run<IEnumerable<TStorage>>(() =>
            {
                var idLeaf = TypeIdentity.FindIdentityLeaf<TStorage>();
                // a=>
                ParameterExpression parameter = Expression.Parameter(typeof(TStorage), "a");
                //a=>a.id
                MemberExpression property = Expression.Property(parameter, idLeaf.Name);

                var lambda = Expression.Lambda<Func<TStorage, TId>>(property, parameter);

                var readResult = DbSetFullTreeAble(readFullTree).Join(
                        FilterResults,
                        lambda,
                        f => f.ResultId,
                        (storage, filter) => new { storage, filter })
                    .Where(s => s.filter.SearchId == searchId)
                    .Skip(offset).Take(size).ToList()
                    .Select(s => s.storage);

                return readResult;
            });
        }

        public override async Task<SearchIndex<TId>> IndexAsync(TId id, string indexCorpus)
        {
            var foundResult = await SearchIndex.FirstOrDefaultAsync(r => r.ResultId.Equals(id));

            if (foundResult != null)
            {
                foundResult.IndexCorpus = indexCorpus;

                return foundResult;
            }

            var index = new SearchIndex<TId>
            {
                IndexCorpus = indexCorpus,
                ResultId = id
            };

            var inserted = await SearchIndex.AddAsync(index.AsMarked<TStorage, TId>());

            if (inserted.State == EntityState.Added)
            {
                return inserted.Entity;
            }

            return null;
        }
    }
}