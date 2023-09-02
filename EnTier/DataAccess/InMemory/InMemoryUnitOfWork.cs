using System;
using System.Collections;
using System.Collections.Generic;
using EnTier.Repositories;
using EnTier.UnitOfWork;

namespace EnTier.DataAccess.InMemory
{
    public class InMemoryUnitOfWork : UnitOfWorkBase
    {
        private static readonly Dictionary<string, object> Repositories = new Dictionary<string, object>();

        public override IEventStreamRepository<TEvent, TEventId, TStreamId> GetStreamRepository<TEvent, TEventId, TStreamId>()
        {
            return new InMemoryEventStreamRepository<TEvent, TEventId, TStreamId>(Essence);
        }

        protected override ICrudRepository<TStorage, TId> CreateDefaultCrudRepository<TStorage, TId>()
        {
            string key = GetKey<TStorage, TId>();
           
            var repository = new InMemoryRepository<TStorage,TId>();
                
            Repositories.Add(key,repository);
            
            return (ICrudRepository<TStorage, TId>) Repositories[key];
        }

        public override void Complete() { }

        protected override ICrudRepository<TStorage, TId> ByPassRepositoryInstantiation<TStorage, TId>()
        {
            string key = GetKey<TStorage, TId>();
            
            if (Repositories.ContainsKey(key))
            {
                return (ICrudRepository<TStorage, TId>) Repositories[key];
            }

            return null;
        }

        protected override void OnDeliveringRepository<TStorage, TId>(ICrudRepository<TStorage, TId> repository)
        {
            string key = GetKey<TStorage, TId>();
            
            if (!Repositories.ContainsKey(key))
            {
                Repositories.Add(key,repository);
            }
        }

        public override void Dispose()
        {
            Repositories.Clear();
        }

        private string GetKey<TStorage, TId>()
        {
            string key = typeof(TStorage).FullName + ":" + typeof(TId).FullName;

            return key;
        }

        public override IDataBoundRepository GetDataBoundRepository<TStorage>()
        {
            var objectsStream = ObjectStreamUnitOfWorkHelper
                .PullOutObjectStreamFromCrudRepositoriesObjectList<TStorage>(Repositories.Values);

            return new ObjectStreamDataBoundRepositoryBase<TStorage>(objectsStream);
        }


        
        
        //TODO: Fix Circular Dependency
        public InMemoryUnitOfWork(EnTierEssence essence) : base(essence)
        {
        }
    }
}