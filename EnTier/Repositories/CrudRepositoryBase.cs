using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Acidmanic.Utilities.Filtering;
using Acidmanic.Utilities.Filtering.Models;
using Acidmanic.Utilities.Reflection;
using EnTier.Extensions;
using EnTier.Models;
using EnTier.Repositories.Attributes;
using EnTier.Utility;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace EnTier.Repositories
{
    public abstract class CrudRepositoryBase<TStorage, TId> : ICrudRepository<TStorage, TId>
        where TStorage : class, new()
    {
        public abstract IEnumerable<TStorage> All();

        public abstract TStorage Update(TStorage value);

        protected abstract TStorage Insert(TStorage value);
        public abstract TStorage Set(TStorage value);
        public abstract TStorage GetById(TId id);
        public abstract IEnumerable<TStorage> Find(Expression<Func<TStorage, bool>> predicate);
        public abstract bool Remove(TStorage value);
        public abstract bool Remove(TId id);
        public abstract Task<IEnumerable<TStorage>> AllAsync();

        public abstract Task<TStorage> UpdateAsync(TStorage value);

        protected abstract Task<TStorage> InsertAsync(TStorage value);
        public abstract Task<TStorage> SetAsync(TStorage value);
        public abstract Task<TStorage> GetByIdAsync(TId id);
        public abstract Task<IEnumerable<TStorage>> FindAsync(Expression<Func<TStorage, bool>> predicate);
        public abstract Task<bool> RemoveAsync(TStorage value);
        public abstract Task<bool> RemoveAsync(TId id);

        public void SetLogger(ILogger logger)
        {
            Logger = logger;
        }

        public abstract Task RemoveExpiredFilterResultsAsync();

        public void RemoveExpiredFilterResults()
        {
            RemoveExpiredFilterResultsAsync().Wait();
        }

        public abstract Task<IEnumerable<FilterResult>> PerformFilterIfNeededAsync(FilterQuery filterQuery,string searchId =null);

        public IEnumerable<FilterResult> PerformFilterIfNeeded(FilterQuery filterQuery,string searchId =null)
        {
            return PerformFilterIfNeededAsync(filterQuery).Result;
        }

        public abstract Task<IEnumerable<TStorage>> ReadChunkAsync(int offset, int size, string searchId);

        public IEnumerable<TStorage> ReadChunk(int offset, int size, string searchId)
        {
            return ReadChunkAsync(offset, size, searchId).Result;
        }
        
        protected ILogger Logger { get; private set; } = NullLogger.Instance;

        public virtual TStorage Add(TStorage value)
        {
            var stripped = StripMarkedSubEntities(value);

            return this.Insert(stripped);
        }

        public Task<TStorage> AddAsync(TStorage value)
        {
            var stripped = StripMarkedSubEntities(value);

            return this.InsertAsync(stripped);
        }


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

                if (property.IsTreatAsLeaf() ||
                    TypeCheck.IsEffectivelyPrimitive(propertyType) ||
                    authorizer.IsAllowed(propertyType))
                {
                    var value = property.GetValue(entity);

                    property.SetValue(clone, value);
                }
            }

            return clone;
        }
    }
}