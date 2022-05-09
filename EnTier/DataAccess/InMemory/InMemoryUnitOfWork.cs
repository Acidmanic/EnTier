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

        protected override ICrudRepository<TStorage, TId> CreateDefaultCrudRepository<TStorage, TId>()
        {
            string key = GetKey<TStorage, TId>();
            
            if (!Repositories.ContainsKey(key))
            {
                var repository = new InMemoryRepository<TStorage,TId>();
                
                Repositories.Add(key,repository);
            }
            return (ICrudRepository<TStorage, TId>) Repositories[key];
        }

        public override void Complete() { }
        
        public override void Dispose()
        {
            Repositories.Clear();
        }

        private string GetKey<TStorage, TId>()
        {
            string key = typeof(TStorage).FullName + ":" + typeof(TId).FullName;

            return key;
        }
    }
}