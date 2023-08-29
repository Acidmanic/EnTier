using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Acidmanic.Utilities.DataTypes;
using Acidmanic.Utilities.Filtering;
using Acidmanic.Utilities.Filtering.Extensions;
using Acidmanic.Utilities.Filtering.Models;
using Acidmanic.Utilities.Reflection;
using Acidmanic.Utilities.Reflection.Extensions;
using EnTier.Repositories;
using Microsoft.EntityFrameworkCore;

namespace EnTier.DataAccess.EntityFramework
{
    public class EntityFrameWorkCrudRepository<TStorage, TId> : CrudRepositoryForwardAsyncBase<TStorage, TId>
        where TStorage : class, new()
    {
        protected DbSet<TStorage> DbSet { get; }
        protected DbSet<FilterResult> FilterResults { get; }

        public EntityFrameWorkCrudRepository(DbSet<TStorage> dbSet, DbSet<FilterResult> filterResults)
        {
            DbSet = dbSet;
            FilterResults = filterResults;
        }

        public override IEnumerable<TStorage> All()
        {
            return DbSet.ToList();
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

        public override TStorage GetById(TId id)
        {
            var found = DbSet.Find(id);

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

        public override Task<IEnumerable<FilterResult>> PerformFilterIfNeededAsync(FilterQuery filterQuery,
            string searchId = null)
        {
            searchId ??= Guid.NewGuid().ToString("N");

            return Task.Run<IEnumerable<FilterResult>>(() =>
            {
                var anyResults = FilterResults.Count(r => r.SearchId == searchId);

                if (anyResults < 1)
                {
                    var expressions = filterQuery.ToExpression<TStorage>();

                    var queryable = DbSet.AsQueryable();

                    foreach (var expression in expressions)
                    {
                        queryable = queryable.Where(expression);
                    }

                    var filterResults = queryable.ToList();

                    var idLeaf = TypeIdentity.FindIdentityLeaf<TStorage>();

                    var expirationTime =
                        TimeStamp.Now.TotalMilliSeconds +
                        typeof(TStorage).GetFilterResultExpirationDurationMilliseconds();

                    foreach (var storage in filterResults)
                    {
                        var filterResult = new FilterResult
                        {
                            SearchId = searchId,
                            ResultId = (long)idLeaf.Evaluator.Read(storage),
                            ExpirationTimeStamp = expirationTime
                        };

                        FilterResults.Add(filterResult);
                    }

                    return FilterResults;
                }

                return new FilterResult[] { };
            });
        }

        public override Task<IEnumerable<TStorage>> ReadChunkAsync(int offset, int size, string searchId)
        {
            return Task.Run<IEnumerable<TStorage>>(() =>
            {
                var idLeaf = TypeIdentity.FindIdentityLeaf<TStorage>();
                // a=>
                ParameterExpression parameter = Expression.Parameter(typeof(TStorage), "a");
                //a=>a.id
                MemberExpression property = Expression.Property(parameter, idLeaf.Name);

                var lambda = Expression.Lambda<Func<TStorage, long>>(property, parameter);

                var readResult = DbSet.Join(
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
    }
}