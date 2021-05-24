using System.Collections.Generic;

namespace EnTier.Services
{
    public interface ICrudService<TEntity, in TDomainId> where TEntity : class, new()
    {
        IEnumerable<TEntity> GetAll();

        TEntity GetById(TDomainId id);

        TEntity Add(TEntity value);

        TEntity Update(TEntity value);
        
        TEntity Update(TDomainId id,TEntity value);

        bool Remove(TEntity value);

        bool RemoveById(TDomainId id);
    }
}