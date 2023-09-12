using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Acidmanic.Utilities.Filtering;
using Acidmanic.Utilities.Filtering.Models;
using EnTier.Models;
using Microsoft.Extensions.Logging;

namespace EnTier.Services
{
    public interface ICrudService<TEntity, in TDomainId> where TEntity : class, new()
    {
        Task<Chunk<TEntity>> ReadSequenceAsync(int offset, int size,
            [AllowNull] string searchId,
            FilterQuery filterQuery,
            [AllowNull] string searchTerm,
            [AllowNull] OrderTerm[] orders = null,
            bool readFullTree = false);

        Chunk<TEntity> ReadSequence(int offset, int size,
            [AllowNull] string searchId,
            FilterQuery filterQuery,
            [AllowNull] string searchTerm,
            [AllowNull] OrderTerm[] orders = null,
            bool readFullTree = false);

        Task<IEnumerable<TEntity>> ReadAllAsync(bool readFullTree = false);

        IEnumerable<TEntity> ReadAll(bool readFullTree = false);

        TEntity ReadById(TDomainId id, bool readFullTree = false);

        Task<TEntity> AddAsync(TEntity value, bool alsoIndex, bool fullTreeIndexing);

        TEntity Add(TEntity value, bool alsoIndex, bool fullTreeIndexing);

        TEntity Update(TEntity value, bool alsoIndex, bool fullTreeIndexing);

        TEntity UpdateById(TDomainId id, TEntity value, bool alsoIndex, bool fullTreeIndexing);

        bool Remove(TEntity value);

        bool RemoveById(TDomainId id);

        void SetLogger(ILogger logger);
    }
}