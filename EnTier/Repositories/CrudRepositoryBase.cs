using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Acidmanic.Utilities.Reflection;
using EnTier.Logging;
using EnTier.Repositories.Attributes;
using EnTier.Utility;
using Microsoft.Extensions.Logging;

namespace EnTier.Repositories
{
    public abstract class CrudRepositoryBase<TStorage, TId> : ICrudRepository<TStorage, TId>
        where TStorage : class, new()
    {
        public abstract IEnumerable<TStorage> All();

        protected abstract TStorage Insert(TStorage value);
        public abstract TStorage Set(TStorage value);
        public abstract TStorage GetById(TId id);
        public abstract IEnumerable<TStorage> Find(Expression<Func<TStorage, bool>> predicate);
        public abstract bool Remove(TStorage value);
        public abstract bool Remove(TId id);
        
        protected ILogger Logger { get; } = EnTierLogging.GetInstance().Logger;

        public virtual TStorage Add(TStorage value)
        {
            var stripped = StripMarkedSubEntities(value);

            return this.Insert(stripped);
        }

        public abstract TStorage Update(TStorage value);

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
                if (TypeCheck.IsEffectivelyPrimitive(property.PropertyType))
                {
                    var value = property.GetValue(entity);

                    property.SetValue(clone, value);
                }
            }

            return clone;
        }


        /// <summary>
        /// This method will strip sub properties regarding the delivered attributes to the caller method. 
        /// </summary>
        /// <param name="entity">object to be stripped</param>
        /// <returns>The clone of given Entity with only primitive members copied</returns>
        protected TEntity StripMarkedSubEntities<TEntity>(TEntity entity)
            where TEntity : class, new()
        {
            var deliveredAttributes = new AttributeHelper().DeliveredAttributes<DataInsertionAttribute>();

            var authorizer = new SubPropertyAuthorizer(deliveredAttributes);

            var type = typeof(TEntity);

            var properties = type.GetProperties();

            var clone = new TEntity();

            foreach (var property in properties)
            {
                var propertyType = property.PropertyType;

                if (TypeCheck.IsEffectivelyPrimitive(propertyType) || authorizer.IsAllowed(propertyType))
                {
                    var value = property.GetValue(entity);

                    property.SetValue(clone, value);
                }
            }

            return clone;
        }
    }
}