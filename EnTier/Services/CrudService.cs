using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Acidmanic.Utilities.Filtering;
using Acidmanic.Utilities.Filtering.Models;
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
using Microsoft.AspNetCore.Mvc;
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


        protected string IndexCorpusOf(object value, bool fullTree)
        {
            var rawCorpus = ObjectUtilities.ExtractAllTexts(value, fullTree);

            var indexCorpus = TransliterationService.Transliterate(rawCorpus);

            return indexCorpus;
        }

        public IEnumerable<TDomain> ReadAll(bool readFullTree = false)
        {
            return ReadAllAsync(readFullTree).Result;
        }

        public virtual async Task<IEnumerable<TDomain>> ReadAllAsync(bool readFullTree = false)
        {
            var repository = UnitOfWork.GetCrudRepository<TStorage, TDomainId>();

            var storages = await repository.AllAsync(readFullTree);

            UnitOfWork.Complete();

            var outgoingStorages = RegulateOutgoing(storages);

            var domains = Mapper.Map<IEnumerable<TDomain>>(outgoingStorages);

            return domains;
        }

        public virtual Chunk<TDomain> ReadSequence(
            int offset, int size,
            [AllowNull] string searchId,
            FilterQuery filterQuery,
            [AllowNull] string searchTerm,
            [AllowNull] OrderTerm[] orderTerms,
            bool readFullTree = false)
        {
            return ReadSequenceAsync(offset, size, searchId, filterQuery, searchTerm, orderTerms, readFullTree).Result;
        }

        public virtual async Task<Chunk<TDomain>> ReadSequenceAsync(
            int offset, int size,
            [AllowNull] string searchId,
            FilterQuery filterQuery,
            [AllowNull] string searchTerm,
            [AllowNull] OrderTerm[] orderTerms,
            bool readFullTree = false)
        {
            var repository = UnitOfWork.GetCrudRepository<TStorage, TDomainId>();

            await repository.RemoveExpiredFilterResultsAsync();

            var searchTerms = TransliterationService.Transliterate(searchTerm ?? "")
                .Split(' ', StringSplitOptions.RemoveEmptyEntries);

            orderTerms ??= new OrderTerm[] { };

            var foundResults = await repository
                .PerformFilterIfNeededAsync(filterQuery, searchId, searchTerms, orderTerms, readFullTree);

            UnitOfWork.Complete();

            var totalCount = foundResults.Count();

            if (totalCount > 0)
            {
                searchId = foundResults.First().SearchId;
            }

            var storages = await repository.ReadChunkAsync(offset, size, searchId, readFullTree);

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

        public virtual TDomain ReadById(TDomainId id, bool readFullTree = false)
        {
            var storageId = Mapper.MapId<TStorageId>(id);

            var storage = UnitOfWork.GetCrudRepository<TStorage, TStorageId>().GetById(storageId, readFullTree);

            var regulatedStorage = RegulateOutgoing(storage);

            if (regulatedStorage == null)
            {
                return null;
            }

            var domain = Mapper.Map<TDomain>(regulatedStorage);

            return domain;
        }

        public virtual TDomain Add(TDomain value, bool alsoIndex, bool fullTreeIndexing)
        {
            return AddAsync(value, alsoIndex, fullTreeIndexing).Result;
        }

        public virtual async Task<TDomain> AddAsync(TDomain value, bool alsoIndex, bool fullTreeIndexing)
        {
            var regulated = RegulateIncoming(value);

            if (regulated)
            {
                var storage = Mapper.Map<TStorage>(regulated.Value);

                var repository = UnitOfWork.GetCrudRepository<TStorage, TStorageId>();

                storage = await repository.AddAsync(storage);

                UnitOfWork.Complete();

                if (alsoIndex)
                {
                    await TryIndex(storage, fullTreeIndexing);
                }

                var domain = Mapper.Map<TDomain>(storage);

                return domain;
            }

            return default;
        }

        private async Task<bool> TryIndex(TStorage storage, bool fullTreeIndexing)
        {
            var allGood = true;

            if (_entityHasId)
            {
                var id = (TStorageId)StorageIdLeaf.Evaluator.Read(storage);

                var repository = UnitOfWork.GetCrudRepository<TStorage, TStorageId>();

                if (fullTreeIndexing)
                {
                    var fullFreeObject = await repository.GetByIdAsync(id, true);

                    storage = fullFreeObject ?? storage;
                }

                var corpus = IndexCorpusOf(storage, fullTreeIndexing);

                var index = await repository.IndexAsync(id, corpus);

                UnitOfWork.Complete();

                if (index == null)
                {
                    Logger.LogError("Unable to index an instance of {Object Type}.", typeof(TStorage).FullName);

                    allGood = false;
                }
                else
                {
                    Logger.LogDebug("Indexed an instance of {ObjectType}, Object Id: {ObjectId}, Index Id: {IndexId}"
                        , typeof(TStorage).FullName, id, index.Id);
                }
            }
            else
            {
                Logger.LogWarning("An Attempt to index an object of type {ObjectType} " +
                                  "been rejected. This entity type does not have an identifier field therefore" +
                                  "it's not possible to use searching and filtering features for it."
                    , typeof(TStorage).FullName);

                allGood = false;
            }

            return allGood;
        }

        public virtual TDomain Update(TDomain value, bool alsoIndex, bool fullTreeIndexing)
        {
            var regulated = RegulateIncoming(value);

            if (regulated)
            {
                var repo = UnitOfWork.GetCrudRepository<TStorage, TDomainId>();

                var regulatedStorage = Mapper.Map<TStorage>(regulated.Value);

                var updated = repo.Update(regulatedStorage);

                UnitOfWork.Complete();

                if (alsoIndex)
                {
                    TryIndex(updated, fullTreeIndexing).Wait();
                }

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

        public virtual TDomain UpdateById(TDomainId id, TDomain value, bool alsoIndex, bool fullTreeIndexing)
        {
            if (_entityHasId)
            {
                DomainIdLeaf.Evaluator.Write(value, id);
            }

            return Update(value, alsoIndex, fullTreeIndexing);
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

        public virtual TDomain UpdateOrInsert(TDomain value, bool alsoIndex, bool fullTreeIndexing)
        {
            return UpdateOrInsertAsync(value, alsoIndex, fullTreeIndexing).Result;
        }

        public virtual async Task<TDomain> UpdateOrInsertAsync(TDomain value, bool alsoIndex, bool fullTreeIndexing)
        {
            var regulated = RegulateIncoming(value);

            if (regulated)
            {
                var repo = UnitOfWork.GetCrudRepository<TStorage, TDomainId>();

                var regulatedStorage = Mapper.Map<TStorage>(regulated.Value);

                var saved = await repo.SetAsync(regulatedStorage);

                UnitOfWork.Complete();

                if (alsoIndex)
                {
                    await TryIndex(saved, fullTreeIndexing);
                }

                if (saved != null)
                {
                    var outgoing = RegulateOutgoing(saved);

                    if (outgoing != null)
                    {
                        var domain = Mapper.Map<TDomain>(outgoing);

                        return domain;
                    }
                }
            }

            return null;
        }

        public virtual Task<bool> TryIndex(TDomain value, bool fullTreeIndexing = false)
        {
            var storage = Mapper.Map<TStorage>(value);

            return TryIndex(storage, fullTreeIndexing);
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