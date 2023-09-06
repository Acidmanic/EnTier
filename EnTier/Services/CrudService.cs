using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Acidmanic.Utilities.Filtering;
using Acidmanic.Utilities.Reflection;
using Acidmanic.Utilities.Reflection.ObjectTree;
using Acidmanic.Utilities.Results;
using EnTier.Contracts;
using EnTier.Mapper;
using EnTier.Models;
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

        protected ITransliterationService TransliterationService { get; }
        
        
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
            TransliterationService = essence.TransliterationService;
        }


        public virtual Chunk<TDomain> GetAll(bool readFullTree = false)
        {
            return GetAllAsync(readFullTree).Result;
        }

        public virtual Task<Chunk<TDomain>> GetAllAsync(bool readFullTree = false)
        {
            return GetAllAsync(0, int.MaxValue,readFullTree);
        }

        public virtual Task<Chunk<TDomain>> GetAllAsync(int offset, int size,bool readFullTree = false)
        {
            return GetAllAsync(offset, size, null, new FilterQuery(),null,readFullTree);
        }

        public virtual  Task<Chunk<TDomain>> GetAllAsync(
            int offset, int size, [AllowNull] string searchId,
            FilterQuery filterQuery,[AllowNull] string searchTerm, bool readFullTree = false)
        {
            
            
            // if (!_entityHasId || !TypeCheck.IsNumerical<TStorageId>())
            // {
            //    return GetAllNoFilterAsync(offset, size, searchId, readFullTree);   
            // }

            return GetAllFilteredAsync(offset, size, searchId, filterQuery, searchTerm, readFullTree);
        }
        
        protected virtual async Task<Chunk<TDomain>> GetAllNoFilterAsync(
            int offset, int size, [AllowNull] string searchId,bool readFullTree = false)
        {
            var repository = UnitOfWork.GetCrudRepository<TStorage, TDomainId>();


            var storages = await repository.AllAsync(readFullTree);

            UnitOfWork.Complete();

            var totalCount = storages.Count();
            
            var outgoingStorages = RegulateOutgoing(storages);

            var domains = Mapper.Map<IEnumerable<TDomain>>(outgoingStorages);

            return new Chunk<TDomain>
            {
                Items = domains,
                Offset = offset,
                Size = size,
                TotalCount = totalCount,
                SearchId = searchId
            };
        }

        protected virtual async Task<Chunk<TDomain>> GetAllFilteredAsync(
            int offset, int size, [AllowNull] string searchId,
            FilterQuery filterQuery, [AllowNull] string searchTerm, bool readFullTree = false)
        {
            var repository = UnitOfWork.GetCrudRepository<TStorage, TDomainId>();

            await repository.RemoveExpiredFilterResultsAsync();
            
            var searchTerms = TransliterationService.Transliterate(searchTerm??"")
                .Split(' ', StringSplitOptions.RemoveEmptyEntries);

            var foundResults = await repository
                .PerformFilterIfNeededAsync(filterQuery,searchId,searchTerms,readFullTree);

            UnitOfWork.Complete();

            var totalCount = foundResults.Count();
            
            if (totalCount>0)
            {
                searchId = foundResults.First().SearchId;
            }
            
            var storages = await repository.ReadChunkAsync(offset, size, searchId,readFullTree);

            var outgoingStorages = RegulateOutgoing(storages);

            var domains = Mapper.Map<IEnumerable<TDomain>>(outgoingStorages);

            return new Chunk<TDomain>
            {
                Items = domains,
                Offset = offset,
                Size = size,
                TotalCount = totalCount,
                SearchId = searchId
            };
        }

        public virtual TDomain GetById(TDomainId id,bool readFullTree = false)
        {
            var storageId = Mapper.MapId<TStorageId>(id);

            var storage = UnitOfWork.GetCrudRepository<TStorage, TStorageId>().GetById(storageId,readFullTree);

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

        public virtual TDomain UpdateById(TDomainId id, TDomain value)
        {
            if (_entityHasId)
            {
                DomainIdLeaf.Evaluator.Write(value, id);
            }

            return Update(value);
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