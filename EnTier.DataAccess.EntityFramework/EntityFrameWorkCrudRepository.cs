using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Acidmanic.Utilities.Reflection;
using Acidmanic.Utilities.Reflection.Extensions;
using EnTier.Extensions;
using EnTier.Query;
using EnTier.Repositories;
using EnTier.Repositories.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;

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

        public override Task PerformFilterIfNeededAsync(FilterQuery filterQuery)
        {
            return Task.Run(() =>
            {
                var hash = filterQuery.Hash();

                var anyResults = FilterResults.Count(r => r.Id == hash);

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
                        DateTime.Now.Ticks +
                        typeof(TStorage).GetFilterResultExpirationTimeSpan().Ticks;

                    foreach (var storage in filterResults)
                    {
                        var filterResult = new FilterResult
                        {
                            Id = hash,
                            ResultId = (long)idLeaf.Evaluator.Read(storage),
                            ExpirationTimeStamp = expirationTime
                        };

                        FilterResults.Add(filterResult);
                    }
                }
            });
        }

        public override Task<IEnumerable<TStorage>> ReadChunkAsync(int offset, int size, string hash)
        {
            return Task.Run<IEnumerable<TStorage>>(() =>
            {
                var idLeaf = TypeIdentity.FindIdentityLeaf<TStorage>();
                // a=>
                ParameterExpression parameter = Expression.Parameter(typeof(TStorage), "a");
                //a=>a.id
                MemberExpression property = Expression.Property(parameter, idLeaf.Name);

                var lambda = Expression.Lambda<Func<TStorage, long>>(property, parameter);

                var readResult = DbSet.LeftJoin(
                        FilterResults, 
                        lambda, 
                        f => f.ResultId,
                        (storage,filter) => storage)
                    .Skip(offset).Take(size).ToList();

                return readResult;
            });
        }
    }
}