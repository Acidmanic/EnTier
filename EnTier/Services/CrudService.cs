using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Acidmanic.Utilities.Reflection;
using Acidmanic.Utilities.Reflection.ObjectTree;
using Acidmanic.Utilities.Results;
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

        private readonly bool _hasRegulator;

        /// <summary>
        /// Using this constructor EnTier will try to instantiate Mapper, UnitOfWork and Regulator.
        /// </summary>
        public CrudService(EnTierEssence essence)
        {
            _entityHasId = StorageIdLeaf != null && DomainIdLeaf != null;

            UnitOfWork = essence.UnitOfWork;
            Mapper = essence.Mapper;
            Regulator = essence.Regulator<TDomain, TStorage>();
            //Just in case!
            Regulator = Regulator ?? new NullDataAccessRegulator<TDomain, TStorage>();
            Logger = essence.Logger;
            _hasRegulator = !(Regulator is NullDataAccessRegulator);
        }


        public virtual IEnumerable<TDomain> GetAll()
        {
            var storages = UnitOfWork.GetCrudRepository<TStorage, TDomainId>().All();

            var outgoingStorages = RegulateOutgoing(storages);

            var domains = Mapper.Map<IEnumerable<TDomain>>(outgoingStorages);

            return domains;
        }


        public virtual TDomain GetById(TDomainId id)
        {
            var storageId = Mapper.MapId<TStorageId>(id);

            var storage = UnitOfWork.GetCrudRepository<TStorage, TStorageId>().GetById(storageId);

            var regulatedStorage = RegulateOutgoing(storage);

            if (regulatedStorage == null)
            {
                return null;
            }

            var domain = Mapper.Map<TDomain>(regulatedStorage);

            return domain;
        }

        public virtual TDomain Add(TDomain value)
        {
            var regulated = RegulateIncoming(value);

            if (regulated)
            {
                var storage = Mapper.Map<TStorage>(regulated.Value);

                storage = UnitOfWork.GetCrudRepository<TStorage, TStorageId>().Add(storage);

                UnitOfWork.Complete();

                var domain = Mapper.Map<TDomain>(storage);

                return domain;
            }

            return default;
        }

        public virtual TDomain Update(TDomain value)
        {
            if (_entityHasId && DomainIdLeaf.Evaluator.Read(value) is TDomainId id)
            {
                return UpdateById(id, value);
            }

            // Not Successful
            return null;
        }

        public virtual TDomain UpdateById(TDomainId id, TDomain value)
        {
            var storageId = Mapper.MapId<TStorageId>(id);

            return UpdateByStorageId(storageId, value);
        }

        private TDomain UpdateByStorageId(TStorageId id, TDomain value)
        {
            var regulated = RegulateIncoming(value);

            if (regulated)
            {
                var repo = UnitOfWork.GetCrudRepository<TStorage, TDomainId>();

                var regulatedStorage = Mapper.Map<TStorage>(regulated.Value);

                var updated = repo.Update(regulatedStorage);

                UnitOfWork.Complete();
                
                if (updated != null)
                {
                    var outgoing = RegulateOutgoing(updated);

                    if (outgoing != null)
                    {
                        var domain = Mapper.Map<TDomain>(outgoing);

                        return domain;
                    }
                }
            }
            return null;
        }

        public virtual bool Remove(TDomain value)
        {

            var regulated = RegulateIncoming(value);

            if (regulated)
            {
                var storage = Mapper.Map<TStorage>(regulated.Value);

                var success = UnitOfWork.GetCrudRepository<TStorage, TStorageId>().Remove(storage);

                if (success)
                {
                    UnitOfWork.Complete();
                }

                return success;    
            }

            return false;
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

        private TStorage RegulateOutgoing(TStorage model)
        {
            if (_hasRegulator)
            {
                var result = Regulator.RegulateOutgoing(model);

                OnRegulation(result);

                return result.Model;
            }

            return model;
        }

        private Result<TDomain> RegulateIncoming(TDomain model)
        {
            if (_hasRegulator)
            {
                var result = Regulator.RegulateIncoming(model);

                OnRegulation(result);

                if (result.Status == RegulationStatus.Ok)
                {
                    return new Result<TDomain>().Succeed(result.Model);
                }

                return new Result<TDomain>().FailAndDefaultValue();
            }

            return new Result<TDomain>().Succeed(model);
        }

        private IEnumerable<TStorage> RegulateOutgoing(IEnumerable<TStorage> models)
        {
            if (_hasRegulator && models != null)
            {
                var regulatedModels = new List<TStorage>();

                foreach (var model in models)
                {
                    var result = Regulator.RegulateOutgoing(model);

                    OnRegulation(result);

                    if (result.Status == RegulationStatus.Ok)
                    {
                        regulatedModels.Add(result.Model);
                    }
                }

                return regulatedModels;
            }

            return models;
        }

        protected virtual void OnRegulation<TModel>(RegulationResult<TModel> result)
        {
            LogNoneOkRegulationResult(result);
        }

        protected void LogNoneOkRegulationResult<TModel>(RegulationResult<TModel> result)
        {
            if (result.Status == RegulationStatus.UnAcceptable)
            {
                Logger.LogCritical("Regulation has rejected the data of type {TModel}.", typeof(TModel).Name);
            }

            if (result.Status == RegulationStatus.Suspicious)
            {
                Logger.LogCritical("Suspicious data of type {TModel} has been detected in regulation.",
                    typeof(TModel).Name);
            }
        }
    }
}