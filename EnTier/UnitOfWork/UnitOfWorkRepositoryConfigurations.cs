using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using EnTier.Repositories;

namespace EnTier.UnitOfWork
{
    public class UnitOfWorkRepositoryConfigurations
    {
        private readonly  Dictionary<Type, Type> _repositoryMap = new Dictionary<Type, Type>();

        public void RegisterCustomRepository<TStorage,TId, TRepository>() where TStorage : class, new()
        {
            this._repositoryMap[typeof(ICrudRepository<TStorage,TId>)] = typeof(TRepository);
        }


        public Type GetRepositoryType<TStorage,TId>() where TStorage : class, new()
        {
            var type = typeof(ICrudRepository<TStorage,TId>);

            if (this._repositoryMap.ContainsKey(type))
            {
                return _repositoryMap[type];
            }
            return null;
        }

        private UnitOfWorkRepositoryConfigurations()
        {
            
        }

        private static UnitOfWorkRepositoryConfigurations instance = null;

        private static object lockObject = new Object();

        public static UnitOfWorkRepositoryConfigurations GetInstance()
        {
            lock (lockObject)
            {
                if (instance == null)
                {
                    instance = new  UnitOfWorkRepositoryConfigurations();
                }
            }
            return instance;
        }

    }
}