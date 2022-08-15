using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace EnTier.Repositories
{
    public interface ICrudRepository<TStorage, TId>
        where TStorage : class, new()
    {
        /// <summary>
        /// Provides all existing instances of Entity
        /// </summary>
        IEnumerable<TStorage> All();

        /// <summary>
        /// Adds new instance of Entity
        /// </summary>
        TStorage Add(TStorage value);

        /// <summary>
        /// Updates an existing instance of entity for its properties
        /// </summary>
        TStorage Update(TStorage value);

        /// <summary>
        /// If the passed value, is an existing Entity, then it's properties,
        /// will be updated, Otherwise, the new value will be added. 
        /// </summary>
        TStorage Set(TStorage value);

        /// <summary>
        /// Finds an existing entity by its Id
        /// </summary>
        TStorage GetById(TId id);

        /// <summary>
        /// Performs a search inside the existing entities.
        /// </summary>
        IEnumerable<TStorage> Find(Expression<Func<TStorage, bool>> predicate);

        /// <summary>
        /// Removes an existing Entity
        /// </summary>
        bool Remove(TStorage value);

        /// <summary>
        /// Removes an existing Entity
        /// </summary>
        bool Remove(TId id);
    }
}