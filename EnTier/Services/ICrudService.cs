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
        Chunk<TEntity> GetAll();

        Task<Chunk<TEntity>> GetAllAsync(int offset, int size,[AllowNull] string searchId , FilterQuery filterQuery);

        Task<Chunk<TEntity>> GetAllAsync(int offset, int size);

        Task<Chunk<TEntity>> GetAllAsync();

        TEntity GetById(TDomainId id);

        TEntity Add(TEntity value);

        TEntity Update(TEntity value);
        
        TEntity UpdateById(TDomainId id,TEntity value);

        bool Remove(TEntity value);

        bool RemoveById(TDomainId id);

        void SetLogger(ILogger logger);
    }
}