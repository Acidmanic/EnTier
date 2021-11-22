using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace EnTier.UnitOfWork
{
    public class UnitOfWorkRepositoryConfigurations
    {
        private readonly  Dictionary<Type, Type> _repositoryMap = new Dictionary<Type, Type>();

        public void RegisterCustomRepository<TAbstraction, TRepository>()
        {
            this._repositoryMap[typeof(TAbstraction)] = typeof(TRepository);
        }


        public Type GetRepositoryType<TAbstraction>()
        {
            var type = typeof(TAbstraction);

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