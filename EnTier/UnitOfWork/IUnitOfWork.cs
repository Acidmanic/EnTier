using System;
using EnTier.Repositories;
using Microsoft.Extensions.Logging;

namespace EnTier.UnitOfWork
{
    public interface IUnitOfWork: IDisposable
    {
        ICrudRepository<TStorage, TId> GetCrudRepository<TStorage, TId>()
            where TStorage : class, new();

        void Complete();

    }
}