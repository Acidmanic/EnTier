using EnTier.EventStreams;
using EnTier.Repositories;
using Microsoft.Extensions.Logging;

namespace EnTier.UnitOfWork
{
    public abstract class UnitOfWorkBase : IUnitOfWork
    {
        protected ILogger Logger { get; private set; }
        protected EnTierEssence Essence { get; private set; }


        public UnitOfWorkBase(EnTierEssence essence)
        {
            Essence = essence;

            Logger = essence.Logger;
        }

        public ICrudRepository<TStorage, TId> GetCrudRepository<TStorage, TId>() where TStorage : class, new()
        {
            ICrudRepository<TStorage, TId> repository = ByPassRepositoryInstantiation<TStorage, TId>();

            if (repository == null)
            {
                repository = Essence.ResolveOrDefault<ICrudRepository<TStorage, TId>>
                    (CreateDefaultCrudRepository<TStorage, TId>);
            }

            if (repository != null)
            {
                repository.SetLogger(Logger);
            }

            OnDeliveringRepository(repository);

            return repository;
        }

        public abstract IEventStreamRepository<TEvent, TEventId, TStreamId> GetStreamRepository<TEvent, TEventId, TStreamId>();

        protected abstract ICrudRepository<TStorage, TId> CreateDefaultCrudRepository<TStorage, TId>()
            where TStorage : class, new();

        /// <summary>
        /// This method will allow the derived classes to bypass Repository creation before <code>UnitOfWorkBase</code>
        /// creates a new instance. Overriding this method will make derived class able to implement caching and etc. 
        /// </summary>
        /// <typeparam name="TStorage">Type of Storage model</typeparam>
        /// <typeparam name="TId">Type of Id Field of Storage model</typeparam>
        /// <returns>returns An instance of <code>ICrudRepository&lt;TStorage, TId&gt;</code> to bypass Repository creation.
        /// Or returns null to let <code>UnitOfWorkBase</code> to create a repository instance.
        /// </returns>
        protected virtual ICrudRepository<TStorage, TId> ByPassRepositoryInstantiation<TStorage, TId>()
            where TStorage : class, new()
        {
            return null;
        }

        protected virtual void OnDeliveringRepository<TStorage, TId>(
            ICrudRepository<TStorage, TId> repository) where TStorage : class, new()
        {
        }

        public abstract void Complete();
        public abstract void Dispose();
    }
}