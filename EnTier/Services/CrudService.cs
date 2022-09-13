using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Acidmanic.Utilities.Reflection;
using Acidmanic.Utilities.Reflection.ObjectTree;
using EnTier.EnTierEssentials;
using EnTier.Exceptions;
using EnTier.Logging;
using EnTier.Mapper;
using EnTier.Regulation;
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
        protected IMapper Mapper { get;  private set;}
        protected IDataAccessRegulator<TDomain, TStorage> Regulator { get;  private set;}


        private readonly bool _unitOfWorkInjected=false;
        private readonly bool _mapperInjected=false;
        private readonly bool _regulatorInjected=false;
        
        protected AccessNode StorageIdLeaf { get; } = TypeIdentity.FindIdentityLeaf<TStorage, TStorageId>();
        protected AccessNode DomainIdLeaf { get; } = TypeIdentity.FindIdentityLeaf<TDomain, TDomainId>();


        private readonly bool _entityHasId;


        protected ILogger Logger { get; } = EnTierLogging.GetInstance().Logger;
        /// <summary>
        /// Using this constructor EnTier will try to instantiate Mapper, UnitOfWork and Regulator.
        /// </summary>
        public CrudService()
        {
            _entityHasId = StorageIdLeaf != null && DomainIdLeaf != null;
        
            var serviceEssentialsProvider = new ServiceEssentialsProvider<TDomain, TStorage, TDomainId, TStorageId>
                (null,null,null);

            UnitOfWork = serviceEssentialsProvider.UnitOfWork;
            Mapper = serviceEssentialsProvider.Mapper;
            Regulator = serviceEssentialsProvider.DataAccessRegulator;

        }
        /// <summary>
        /// You can override di-registered and default values for UnitOfWork, Mapper and Regulators using this
        /// constructor. Also you can pass null for each one you needed EnTier to creates an instance for.
        /// These values will be override by 
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <param name="mapper"></param>
        /// <param name="regulator"></param>
        public CrudService(IUnitOfWork unitOfWork, IMapper mapper, IDataAccessRegulator<TDomain, TStorage> regulator)
        {
            _unitOfWorkInjected = unitOfWork != null;
            _mapperInjected = unitOfWork != null;
            _regulatorInjected = unitOfWork != null;
            
            _entityHasId = StorageIdLeaf != null && DomainIdLeaf != null;
        
            var serviceEssentialsProvider = new ServiceEssentialsProvider<TDomain, TStorage, TDomainId, TStorageId>
                (mapper,unitOfWork,regulator);

            UnitOfWork = serviceEssentialsProvider.UnitOfWork;
            Mapper = serviceEssentialsProvider.Mapper;
            Regulator = serviceEssentialsProvider.DataAccessRegulator;

        }

        /// <summary>
        /// Controller will call this and set only the values that client code has not injected
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <param name="mapper"></param>
        /// <param name="regulator"></param>
        /// <returns></returns>
        internal CrudService<TDomain, TStorage, TDomainId, TStorageId> InitializeEssentials(IUnitOfWork unitOfWork, IMapper mapper, IDataAccessRegulator<TDomain, TStorage> regulator)
        {

            if (!_unitOfWorkInjected)
            {
                UnitOfWork = unitOfWork;    
            }

            if (!_mapperInjected)
            {
                Mapper = mapper;    
            }

            if (!_regulatorInjected)
            {
                Regulator = regulator;    
            }
            return this;
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
    }
}