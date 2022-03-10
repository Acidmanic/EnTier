using System;
using System.Linq;
using System.Reflection;
using EnTier.Exceptions;
using EnTier.Repositories;

namespace EnTier.UnitOfWork
{
    public abstract class UnitOfWorkBase : IUnitOfWork
    {
        public ICrudRepository<TStorage, TId> GetCrudRepository<TStorage, TId>() where TStorage : class, new()
        {
            var customRepositoryType = UnitOfWorkRepositoryConfigurations.GetInstance()
                .GetRepositoryType<ICrudRepository<TStorage, TId>>();

            if (customRepositoryType != null)
            {
                //Return Custom Repository
                return GetCustomRepository<TStorage, TId>(customRepositoryType);
            }
            var repository = CreateDefaultCrudRepository<TStorage, TId>();
            
            return repository;
        }

        protected abstract ICrudRepository<TStorage, TId> CreateDefaultCrudRepository<TStorage, TId>()
            where TStorage : class, new();

        public TCustomCrudRepository GetCrudRepository<TStorage, TId, TCustomCrudRepository>()
            where TStorage : class, new() where TCustomCrudRepository : ICrudRepository<TStorage, TId>
        {
            var repositoryType = typeof(TCustomCrudRepository);

            return (TCustomCrudRepository) GetCustomRepository<TStorage, TId>(repositoryType);
        }
        
        private ICrudRepository<TStorage,TId> GetCustomRepository<TStorage, TId>(Type repositoryType)
            where TStorage : class, new()
        {
            
            var repoConstructor = GetConstructor(repositoryType);

            if (repoConstructor == null)
            {
                throw new InvalidConstructorException();
            }

            object[] parameterValues = ProvideConstructorParameters(repoConstructor);
            
            var repository = repoConstructor.Invoke(parameterValues);

            return (ICrudRepository<TStorage,TId>) repository;
        }

        private object[] ProvideConstructorParameters(ConstructorInfo repoConstructor)
        {
            var parameters = repoConstructor.GetParameters();
            
            var dbSetTypes = repoConstructor.GetParameters()
                .Select(p => p.ParameterType).ToList();

            var parameterValues = new object[dbSetTypes.Count];

            for (int i = 0; i < parameterValues.Length; i++)
            {
                parameterValues[i] = ProvideConstructorParameter(parameters[i].ParameterType);
            }

            return parameterValues;
        }

        protected virtual bool IsConstructorAcceptable(ConstructorInfo constructor)
        {
            return constructor.IsAbstract == false &&
                   constructor.IsPrivate == false &&
                   constructor.GetParameters().Length == 0;
        }
        
        private ConstructorInfo GetConstructor(Type repoType)
        {
            var constructors = repoType.GetConstructors();

            var dbSetOnlyConstructors = constructors.Where(IsConstructorAcceptable);

            var setOnlyConstructors = dbSetOnlyConstructors as ConstructorInfo[] ?? dbSetOnlyConstructors.ToArray();

            if (setOnlyConstructors.Length > 0)
            {
                var theConstructor = setOnlyConstructors[0];

                int longest = theConstructor.GetParameters().Length;

                foreach (var c in setOnlyConstructors)
                {
                    var len = c.GetParameters().Length;

                    if (len > longest)
                    {
                        longest = len;

                        theConstructor = c;
                    }
                }

                return theConstructor;
            }

            return null;
        }

        protected virtual object ProvideConstructorParameter(Type parameterType)
        {
            return null;
        }
        public abstract void Complete();
    }
}