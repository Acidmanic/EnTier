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
                repository = Essence.ResolveOrDefault(CreateDefaultCrudRepository<TStorage, TId>);
            }

            OnDeliveringRepository(repository);

            return repository;
        }

        protected abstract ICrudRepository<TStorage, TId> CreateDefaultCrudRepository<TStorage, TId>()
            where TStorage : class, new();

        //
        // private ICrudRepository<TStorage, TId> GetCustomRepository<TStorage, TId>(Type repositoryType)
        //     where TStorage : class, new()
        // {
        //     var repoConstructor = GetConstructor(repositoryType);
        //
        //     if (repoConstructor == null)
        //     {
        //         throw new InvalidConstructorException();
        //     }
        //
        //     object[] parameterValues = ProvideConstructorParameters(repoConstructor);
        //
        //     var repository = repoConstructor.Invoke(parameterValues);
        //
        //     return (ICrudRepository<TStorage, TId>) repository;
        // }
        //
        // private object[] ProvideConstructorParameters(ConstructorInfo repoConstructor)
        // {
        //     var parameters = repoConstructor.GetParameters();
        //
        //     var dbSetTypes = repoConstructor.GetParameters()
        //         .Select(p => p.ParameterType).ToList();
        //
        //     var parameterValues = new object[dbSetTypes.Count];
        //
        //     for (int i = 0; i < parameterValues.Length; i++)
        //     {
        //         parameterValues[i] = ProvideConstructorParameter(parameters[i].ParameterType);
        //     }
        //
        //     return parameterValues;
        // }
        //
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
        //
        // protected virtual bool IsConstructorAcceptable(ConstructorInfo constructor)
        // {
        //     return constructor.IsAbstract == false &&
        //            constructor.IsPrivate == false &&
        //            constructor.GetParameters().Length == 0;
        // }
        //
        // private ConstructorInfo GetConstructor(Type repoType)
        // {
        //     var constructors = repoType.GetConstructors();
        //
        //     var dbSetOnlyConstructors = constructors.Where(IsConstructorAcceptable);
        //
        //     var setOnlyConstructors = dbSetOnlyConstructors as ConstructorInfo[] ?? dbSetOnlyConstructors.ToArray();
        //
        //     if (setOnlyConstructors.Length > 0)
        //     {
        //         var theConstructor = setOnlyConstructors[0];
        //
        //         int longest = theConstructor.GetParameters().Length;
        //
        //         foreach (var c in setOnlyConstructors)
        //         {
        //             var len = c.GetParameters().Length;
        //
        //             if (len > longest)
        //             {
        //                 longest = len;
        //
        //                 theConstructor = c;
        //             }
        //         }
        //
        //         return theConstructor;
        //     }
        //
        //     return null;
        // }
        //
        // protected virtual object ProvideConstructorParameter(Type parameterType)
        // {
        //     return null;
        // }

        public abstract void Complete();
        public abstract void Dispose();
    }
}