using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Acidmanic.Utilities.Filtering;
using EnTier.Models;
using Microsoft.Extensions.Logging;

namespace EnTier.Services
{
    public interface ICrudService<TEntity, in TDomainId> where TEntity : class, new()
    {
        Chunk<TEntity> GetAll(bool readFullTree = false);

        Task<Chunk<TEntity>> GetAllAsync(int offset, int size,[AllowNull] string searchId , FilterQuery filterQuery,bool readFullTree = false);

        Task<Chunk<TEntity>> GetAllAsync(int offset, int size,bool readFullTree = false);

        Task<Chunk<TEntity>> GetAllAsync(bool readFullTree = false);

        TEntity GetById(TDomainId id,bool readFullTree = false);

        TEntity Add(TEntity value);

        TEntity Update(TEntity value);
        
        TEntity UpdateById(TDomainId id,TEntity value);

        bool Remove(TEntity value);

        bool RemoveById(TDomainId id);

        void SetLogger(ILogger logger);
    }
}