using System.Collections.Generic;

namespace EnTier.Services
{
    public interface ICrudService<TEntity, in TId> where TEntity : class, new()
    {
        IEnumerable<TEntity> GetAll();

        TEntity GetById(TId id);

        TEntity Add(TEntity value);

        TEntity Update(TEntity value);

        bool Remove(TEntity value);

        bool RemoveById(TId id);
    }
}