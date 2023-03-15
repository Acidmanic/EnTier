using System;
using EnTier.Repositories;

namespace EnTier.UnitOfWork
{

   
    public interface IUnitOfWork: IDisposable
    {
        ICrudRepository<TStorage, TId> GetCrudRepository<TStorage, TId>()
            where TStorage : class, new();

        IEventStreamRepository<TEvent, TEventId, TStreamId> GetStreamRepository<TEvent, TEventId, TStreamId>();

        void Complete();

    }
}