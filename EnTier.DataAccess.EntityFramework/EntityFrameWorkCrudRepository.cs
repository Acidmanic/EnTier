using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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

        public EntityFrameWorkCrudRepository(DbSet<TStorage> dbSet)
        {
            DbSet = dbSet;
        }

        public override IEnumerable<TStorage> All()
        {
            return DbSet;
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
                        value.CopyInto(found,idLeaf.GetFullName());

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
    }
}