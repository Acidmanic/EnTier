using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Acidmanic.Utilities.Reflection;
using Acidmanic.Utilities.Reflection.ObjectTree;
using EnTier.Exceptions;
using EnTier.Mapper;
using EnTier.Regulation;
using EnTier.Repositories;
using EnTier.UnitOfWork;
using EnTier.Utility;
using Microsoft.Extensions.Logging;

namespace EnTier.Services
{
    public class CrudService<TDomain, TStorage, TDomainId, TStorageId> : ICrudService<TDomain, TDomainId>
        where TDomain : class, new()
        where TStorage : class, new()
    {
        protected IUnitOfWork UnitOfWork { get; private set; }
        protected IMapper Mapper { get; private set; }
        protected IDataAccessRegulator<TDomain, TStorage> Regulator { get; private set; }
        protected ILogger Logger { get; private set; }


        protected AccessNode StorageIdLeaf { get; } = TypeIdentity.FindIdentityLeaf<TStorage, TStorageId>();
        protected AccessNode DomainIdLeaf { get; } = TypeIdentity.FindIdentityLeaf<TDomain, TDomainId>();


        private readonly bool _entityHasId;


        /// <summary>
        /// Using this constructor EnTier will try to instantiate Mapper, UnitOfWork and Regulator.
        /// </summary>
        public CrudService(EnTierEssence essence)
        {
            _entityHasId = StorageIdLeaf != null && DomainIdLeaf != null;

            UnitOfWork = essence.UnitOfWork;
            Mapper = essence.Mapper;
            Regulator = essence.Regulator<TDomain, TStorage>();
            Logger = essence.Logger;
        }


        public virtual IEnumerable<TDomain> GetAll()
        {
            var storages = UnitOfWork.GetCrudRepository<TStorage, TDomainId>().All();

            var domains = Mapper.Map<IEnumerable<TDomain>>(storages);

            return domains;
        }

        public virtual TDomain GetById(TDomainId id)
        {
            var storageId = Mapper.MapId<TStorageId>(id);

            var storage = UnitOfWork.GetCrudRepository<TStorage, TStorageId>().GetById(storageId);

            if (storage == null)
            {
                return null;
            }

            var domain = Mapper.Map<TDomain>(storage);

            return domain;
        }

        private TStorage Regulate(TDomain value)
        {
            TStorage regulatedStorage = default;

            if (Regulator is NullDataAccessRegulator)
            {
                regulatedStorage = Mapper.Map<TStorage>(value);
            }
            else
            {
                var regulationResult = Regulator.Regulate(value);

                if (regulationResult.Status != RegulationStatus.UnAcceptable)
                {
                    regulatedStorage = regulationResult.Storage;
                }
                else
                {
                    throw new UnAcceptableModelException();
                }
            }

            return regulatedStorage;
        }

        public virtual TDomain Add(TDomain value)
        {
            var storage = Regulate(value);

            storage = UnitOfWork.GetCrudRepository<TStorage, TStorageId>().Add(storage);

            UnitOfWork.Complete();

            var domain = Mapper.Map<TDomain>(storage);

            return domain;
        }

        public virtual TDomain Update(TDomain value)
        {
            if (_entityHasId && DomainIdLeaf.Evaluator.Read(value) is TDomainId id)
            {
                return Update(id, value);
            }

            // Not Successful
            return null;
        }

        public virtual TDomain Update(TDomainId id, TDomain value)
        {
            var storageId = Mapper.MapId<TStorageId>(id);

            return Update(storageId, value);
        }

        private TDomain Update(TStorageId id, TDomain value)
        {
            Regulate(value);

            var repo = UnitOfWork.GetCrudRepository<TStorage, TDomainId>();

            Expression<Func<TStorage, bool>> selector = s =>
                _entityHasId && (StorageIdLeaf.Evaluator.Read(s).Equals(id));

            var found = repo.Find(selector).FirstOrDefault();

            if (found != null)
            {
                var storageUpdate = Mapper.Map<TStorage>(value);

                repo.Set(storageUpdate);

                UnitOfWork.Complete();

                return value;
            }

            return null;
        }

        public virtual bool Remove(TDomain value)
        {
            var storage = Mapper.Map<TStorage>(value);

            var success = UnitOfWork.GetCrudRepository<TStorage, TStorageId>().Remove(storage);

            if (success)
            {
                UnitOfWork.Complete();
            }

            return success;
        }

        public virtual bool RemoveById(TDomainId id)
        {
            var storageId = Mapper.MapId<TStorageId>(id);

            var success = UnitOfWork.GetCrudRepository<TStorage, TStorageId>().Remove(storageId);

            if (success)
            {
                UnitOfWork.Complete();
            }

            return success;
        }


        protected DisposableFetchRepositoryResult<TCustom, TStorage, TStorageId> FetchCustomRepository<TCustom>()
            where TCustom : ICrudRepository<TStorage, TStorageId>
        {
            return new DisposableFetchRepositoryResult<TCustom, TStorage, TStorageId>(UnitOfWork, Logger);
        }

        public void SetLogger(ILogger logger)
        {
            Logger = logger;
        }
    }
}