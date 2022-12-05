using System;
using System.Collections.Generic;
using EnTier.Repositories;

namespace EnTier.UnitOfWork
{
    
    public class CustomRepositoryRegistry
    {
        private readonly  Dictionary<Type, Type> _repositoryMap = new Dictionary<Type, Type>();

        public void RegisterCustomRepository<TStorage,TId, TRepository>() where TStorage : class, new()
        {
            this._repositoryMap[typeof(ICrudRepository<TStorage,TId>)] = typeof(TRepository);
        }

        public Type GetRepositoryType<TStorage,TId>() where TStorage : class, new()
        {
            var type = typeof(ICrudRepository<TStorage,TId>);

            return GetRepositoryType(type);
        }

        public Type GetRepositoryType(Type abstraction)
        {
            if (this._repositoryMap.ContainsKey(abstraction))
            {
                return _repositoryMap[abstraction];
            }
            return null;
        }

        public bool Contains(Type abstraction)
        {
            return _repositoryMap.ContainsKey(abstraction);
        }

        public bool Contains<TStorage, TId>()where TStorage : class, new()
        {
            var type = typeof(ICrudRepository<TStorage,TId>);

            return Contains(type);
        }
    }
}