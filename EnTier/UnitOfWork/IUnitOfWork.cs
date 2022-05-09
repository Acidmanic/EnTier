using System;
using EnTier.Repositories;

namespace EnTier.UnitOfWork
{
    public interface IUnitOfWork: IDisposable
    {
        ICrudRepository<TStorage, TId> GetCrudRepository<TStorage, TId>()
            where TStorage : class, new();

        TCustomCrudRepository GetCrudRepository<TStorage, TId, TCustomCrudRepository>()
            where TStorage : class, new()
            where TCustomCrudRepository : ICrudRepository<TStorage, TId>;
        
        void Complete();
    }
}