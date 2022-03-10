using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using EnTier.Repositories;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Serialization;

namespace EnTier.DataAccess.EntityFramework
{
    public class EntityFrameWorkCrudRepository<TStorage, TId> : ICrudRepository<TStorage, TId>
        where TStorage : class, new()
    {
        protected DbSet<TStorage> DbSet { get; }

        public EntityFrameWorkCrudRepository(DbSet<TStorage> dbSet)
        {
            DbSet = dbSet;
        }

        public virtual IEnumerable<TStorage> All()
        {
            return DbSet;
        }

        public virtual TStorage Add(TStorage value)
        {
            var entry = DbSet.Add(value);

            if (entry.State == EntityState.Added)
            {
                return entry.Entity;
            }

            return default;
        }

        public virtual TStorage GetById(TId id)
        {
            var found = DbSet.Find(id);

            return found;
        }

        public virtual IEnumerable<TStorage> Find(Expression<Func<TStorage, bool>> predicate)
        {
            var isIncluded = predicate.Compile();

            return DbSet.Where(isIncluded).ToList();
        }

        public virtual bool Remove(TStorage value)
        {
            return DbSet.Remove(value).State == EntityState.Deleted;
        }

        public virtual bool Remove(TId id)
        {
            var entity = GetById(id);

            if (entity != null)
            {
                return DbSet.Remove(entity).State == EntityState.Deleted;
            }
            return false;
        }
    }
}