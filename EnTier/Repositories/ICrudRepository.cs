using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Acidmanic.Utilities.Filtering;
using Acidmanic.Utilities.Filtering.Models;
using EnTier.Models;
using Microsoft.Extensions.Logging;

namespace EnTier.Repositories
{
    public interface ICrudRepository<TStorage, TId>
        where TStorage : class, new()
    {
        /// <summary>
        /// Provides all existing instances of Entity
        /// </summary>
        IEnumerable<TStorage> All(bool readFullTree = false);

        /// <summary>
        /// Finds an existing entity by its Id
        /// </summary>
        TStorage GetById(TId id,bool readFullTree = false);

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

        //>>>>>>>>>>>>>>>>

        /// <summary>
        /// Provides all existing instances of Entity
        /// </summary>
        Task<IEnumerable<TStorage>> AllAsync(bool readFullTree = false);

        /// <summary>
        /// Adds new instance of Entity
        /// </summary>
        Task<TStorage> AddAsync(TStorage value);

        /// <summary>
        /// Updates an existing instance of entity for its properties
        /// </summary>
        Task<TStorage> UpdateAsync(TStorage value);

        /// <summary>
        /// If the passed value, is an existing Entity, then it's properties,
        /// will be updated, Otherwise, the new value will be added. 
        /// </summary>
        Task<TStorage> SetAsync(TStorage value);

        /// <summary>
        /// Finds an existing entity by its Id
        /// </summary>
        Task<TStorage> GetByIdAsync(TId id,bool readFullTree = false);

        /// <summary>
        /// Performs a search inside the existing entities.
        /// </summary>
        Task<IEnumerable<TStorage>> FindAsync(Expression<Func<TStorage, bool>> predicate);

        /// <summary>
        /// Removes an existing Entity
        /// </summary>
        Task<bool> RemoveAsync(TStorage value);

        /// <summary>
        /// Removes an existing Entity
        /// </summary>
        Task<bool> RemoveAsync(TId id);

        /// <summary>
        /// Sets the internal repository logger
        /// </summary>
        /// <param name="logger"></param>
        void SetLogger(ILogger logger);

// Filter Support

        /// <summary>
        /// This Method will clear expired FilterResult data from data source.
        /// </summary>
        Task RemoveExpiredFilterResultsAsync();

        /// <summary>
        /// This Method will clear expired FilterResult data from data source.
        /// </summary>
        void RemoveExpiredFilterResults();

        /// <summary>
        /// This method, Checks if given filter is not performed. If so, then
        /// it will perform the given filter and store the result into FilterResult data source
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<FilterResult<TId>>> PerformFilterIfNeededAsync(FilterQuery filterQuery, 
            string searchId = null,
            string[] searchTerms = null,
            bool readFullTree = false);

        /// <summary>
        /// This method, Checks if given filter is not performed. If so, then
        /// it will perform the given filter and store the result into FilterResult data source
        /// </summary>
        /// <returns></returns>
        IEnumerable<FilterResult<TId>> PerformFilterIfNeeded(FilterQuery filterQuery,
            string searchId = null,
            string[] searchTerms = null,
            bool readFullTree = false);

        /// <summary>
        /// This method will read a chunk of storage data by skipping 'offset' items and picking 'size' items.
        /// This method uses the FilterResults data so For this method to work, it's necessary that the filter
        /// already has been performed. 
        /// </summary>
        /// <param name="offset">Number if results to be skipped</param>
        /// <param name="size">Maximum number of results to be read</param>
        /// <param name="searchId">The hash of filter-query which whom it's results are being read.</param>
        /// <param name="readFullTree">If True, the method will try to read and assemble all sub-entities for each item.</param>
        /// <returns>The asked chunk of results, if found any.</returns>
        Task<IEnumerable<TStorage>> ReadChunkAsync(int offset, int size, string searchId,bool readFullTree = false);

        /// <summary>
        /// This method will read a chunk of storage data by skipping 'offset' items and picking 'size' items.
        /// This method uses the FilterResults data so For this method to work, it's necessary that the filter
        /// already has been performed. 
        /// </summary>
        /// <param name="offset">Number if results to be skipped</param>
        /// <param name="size">Maximum number of results to be read</param>
        /// <param name="readFullTree">If True, the method will try to read and assemble all sub-entities for each item.</param>
        /// <param name="searchId">The hash of filter-query which whom it's results are being read.</param>
        /// <returns>The asked chunk of results, if found any.</returns>
        IEnumerable<TStorage> ReadChunk(int offset, int size, string searchId,bool readFullTree = false);

        SearchIndex<TId> Index(TId id,string indexCorpus);
        
        Task<SearchIndex<TId>> IndexAsync(TId id,string indexCorpus);
    }
}