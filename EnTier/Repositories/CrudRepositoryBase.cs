using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace EnTier.Repositories
{
    public abstract class CrudRepositoryBase<TStorage, TId> : ICrudRepository<TStorage, TId>
        where TStorage : class, new()
    {
        public abstract IEnumerable<TStorage> All();
        public abstract TStorage Add(TStorage value);
        public abstract TStorage GetById(TId id);
        public abstract IEnumerable<TStorage> Find(Expression<Func<TStorage, bool>> predicate);
        public abstract bool Remove(TStorage value);
        public abstract bool Remove(TId id);

        /// <summary>
        /// This method, takes an entity, and returns a clone with only primitive members copied.
        /// It is useful as a pre-check before inserts.  
        /// </summary>
        /// <param name="entity">object to be stripped</param>
        /// <returns>The clone of given Entity with only primitive members copied</returns>
        protected TEntity StripNonPrimitives<TEntity>(TEntity entity)
            where TEntity : class, new()
        {
            var type = typeof(TEntity);

            var properties = type.GetProperties();
            
            var clone = new TEntity();

            foreach (var property in properties)
            {
                if (property.PropertyType.IsPrimitive)
                {
                    var value = property.GetValue(entity);

                    property.SetValue(clone, value);
                }
            }
            return clone;
        }
    }
}