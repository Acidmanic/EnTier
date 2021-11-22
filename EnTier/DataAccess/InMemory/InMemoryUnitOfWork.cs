using System;
using System.Collections;
using System.Collections.Generic;
using EnTier.Repositories;
using EnTier.UnitOfWork;

namespace EnTier.DataAccess.InMemory
{
    public class InMemoryUnitOfWork : IUnitOfWork
    {
        private static readonly Dictionary<string, object> Repositories = new Dictionary<string, object>();

        public ICrudRepository<TStorage, TId> GetCrudRepository<TStorage, TId>() where TStorage : class, new()
        {
            string key = GetKey<TStorage, TId>();

            if (!Repositories.ContainsKey(key))
            {
                var repository = new InMemoryRepository<TStorage,TId>();
                
                Repositories.Add(key,repository);
            }
            return (ICrudRepository<TStorage, TId>) Repositories[key];
        }

        public TCustomCrudRepository GetCrudRepository<TStorage, TId, TCustomCrudRepository>()
            where TStorage : class, new()
            where TCustomCrudRepository:ICrudRepository<TStorage,TId>
        {
            var repoType = typeof(TCustomCrudRepository);

            var repository  = repoType.GetConstructor(new Type[]{})
                .Invoke(new object[]{});
            
            return (TCustomCrudRepository) repository;
        }

        public void Complete() { }

        private string GetKey<TStorage, TId>()
        {
            string key = typeof(TStorage).FullName + ":" + typeof(TId).FullName;

            return key;
        }
    }
}