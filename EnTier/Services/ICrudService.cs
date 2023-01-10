using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace EnTier.Services
{
    public interface ICrudService<TEntity, in TDomainId> where TEntity : class, new()
    {
        IEnumerable<TEntity> GetAll();

        TEntity GetById(TDomainId id);

        TEntity Add(TEntity value);

        TEntity Update(TEntity value);
        
        TEntity UpdateById(TDomainId id,TEntity value);

        bool Remove(TEntity value);

        bool RemoveById(TDomainId id);

        void SetLogger(ILogger logger);
    }
}