using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Acidmanic.Utilities.DataTypes;
using Acidmanic.Utilities.Filtering;
using Acidmanic.Utilities.Filtering.Models;
using Acidmanic.Utilities.Reflection;
using EnTier.DataAccess.Meadow.GenericCrudRequests;
using EnTier.DataAccess.Meadow.GenericFilteringRequests;
using EnTier.DataAccess.Meadow.GenericFilteringRequests.Models;
using EnTier.Models;
using EnTier.Repositories;
using Meadow;
using Meadow.Configuration;
using Meadow.Requests;
using Microsoft.Extensions.Logging;

namespace EnTier.DataAccess.Meadow
{
    public class MeadowCrudRepository<TStorage, TId> : CrudRepositoryBase<TStorage, TId>
        where TStorage : class, new()
    {
        public MeadowCrudRepository(MeadowConfiguration configuration)
        {
            Configuration = configuration;
        }

        private MeadowConfiguration Configuration { get; }

        private void ErrorCheck(MeadowRequest response)
        {
            if (response.Failed)
            {
                Logger.LogError(response.FailureException, "Meadow Request Failed.");
            }
        }


        public override IEnumerable<TStorage> All()
        {
            var request = new ReadAllStorageRequest<TStorage>();

            var engine = GetEngine();

            var response = engine.PerformRequest(request);

            ErrorCheck(response);

            return response.FromStorage;
        }


        protected override TStorage Insert(TStorage value)
        {
            var request = new InsertStorageRequest<TStorage>();

            request.ToStorage = value;

            var engine = GetEngine();

            var response = engine.PerformRequest(request);

            ErrorCheck(response);

            var result = response.FromStorage.FirstOrDefault();

            return result;
        }

        public override TStorage Update(TStorage value)
        {
            var request = new UpdateStorageRequest<TStorage> { ToStorage = value };

            var engine = GetEngine();

            var response = engine.PerformRequest(request);

            ErrorCheck(response);

            var result = response.FromStorage.FirstOrDefault();

            return result;
        }

        public override TStorage Set(TStorage value)
        {
            var request = new SaveStorageRequest<TStorage> { ToStorage = value };

            var engine = GetEngine();

            var response = engine.PerformRequest(request);

            ErrorCheck(response);

            var result = response.FromStorage.FirstOrDefault();

            return result;
        }

        public override TStorage GetById(TId id)
        {
            var request = new ReadByIdStorageRequest<TStorage, TId>()
            {
                Id = id
            };

            var engine = GetEngine();

            var response = engine.PerformRequest(request);

            ErrorCheck(response);

            return response.FromStorage.FirstOrDefault();
        }

        public override IEnumerable<TStorage> Find(Expression<Func<TStorage, bool>> predicate)
        {
            var searchTerm = predicate.Compile();

            return All().Where(searchTerm);
        }

        public override bool Remove(TStorage value)
        {
            var id = TryReadId(value);

            return Remove(id);
        }

        public override bool Remove(TId id)
        {
            var request = new RemoveByIdStorage<TStorage, TId>
            {
                Id = id
            };

            var engine = GetEngine();

            var response = engine.PerformRequest(request);

            ErrorCheck(response);

            var result = response.FromStorage.FirstOrDefault();

            return result != null && result.Success;
        }

        public override async Task<IEnumerable<TStorage>> AllAsync()
        {
            var request = new ReadAllStorageRequest<TStorage>();

            var engine = GetEngine();

            var response = await engine.PerformRequestAsync(request);

            ErrorCheck(response);

            return response.FromStorage;
        }

        public override async Task<TStorage> UpdateAsync(TStorage value)
        {
            var request = new UpdateStorageRequest<TStorage> { ToStorage = value };

            var engine = GetEngine();

            var response = await engine.PerformRequestAsync(request);

            ErrorCheck(response);

            var result = response.FromStorage.FirstOrDefault();

            return result;
        }

        public override async Task<TStorage> SetAsync(TStorage value)
        {
            var request = new SaveStorageRequest<TStorage> { ToStorage = value };

            var engine = GetEngine();

            var response = await engine.PerformRequestAsync(request);

            ErrorCheck(response);

            var result = response.FromStorage.FirstOrDefault();

            return result;
        }

        public override async Task<TStorage> GetByIdAsync(TId id)
        {
            var request = new ReadByIdStorageRequest<TStorage, TId>()
            {
                Id = id
            };

            var engine = GetEngine();

            var response = await engine.PerformRequestAsync(request);

            ErrorCheck(response);

            return response.FromStorage.FirstOrDefault();
        }

        public override async Task<IEnumerable<TStorage>> FindAsync(Expression<Func<TStorage, bool>> predicate)
        {
            var searchTerm = predicate.Compile();

            var all = await AllAsync();

            return all.Where(searchTerm);
        }

        public override Task<bool> RemoveAsync(TStorage value)
        {
            var id = TryReadId(value);

            return RemoveAsync(id);
        }

        public override async Task<bool> RemoveAsync(TId id)
        {
            var request = new RemoveByIdStorage<TStorage, TId>
            {
                Id = id
            };

            var engine = GetEngine();

            var response = await engine.PerformRequestAsync(request);

            ErrorCheck(response);

            var result = response.FromStorage.FirstOrDefault();

            return result != null && result.Success;
        }

        protected override async Task<TStorage> InsertAsync(TStorage value)
        {
            var request = new InsertStorageRequest<TStorage>();

            request.ToStorage = value;

            var engine = GetEngine();

            var response = await engine.PerformRequestAsync(request);

            ErrorCheck(response);

            var result = response.FromStorage.FirstOrDefault();

            return result;
        }

        protected MeadowEngine GetEngine()
        {
            var engine = new MeadowEngine(Configuration);

            MeadowEngine.UseLogger(Logger);

            return engine;
        }

        protected TId TryReadId(TStorage value)
        {
            try
            {
                var type = typeof(TStorage);

                var leaf = TypeIdentity.FindIdentityLeaf(type);

                if (leaf != null)
                {
                    return (TId)leaf.Evaluator.Read(value);
                }
            }
            catch (Exception e)
            {
                Logger.LogError(e, "There was an error finding Id filed for given model");
            }

            return default;
        }


        public override async Task RemoveExpiredFilterResultsAsync()
        {
            var request = new RemoveExpiredFilterResultsRequest(TimeStamp.Now);

            var engine = GetEngine();

            var response = await engine.PerformRequestAsync(request);

            ErrorCheck(response);
        }

        public override async Task<IEnumerable<FilterResult>> PerformFilterIfNeededAsync(FilterQuery filterQuery,string searchId = null)
        {
            var request = new PerformFilterIfNeededRequest<TStorage>(filterQuery,searchId);

            var engine = GetEngine();

            var response = await engine.PerformRequestAsync(request);

            ErrorCheck(response);

            return request.FromStorage;
        }

        public override async Task<IEnumerable<TStorage>> ReadChunkAsync(int offset, int size, string searchId)
        {
            var request = new ReadChunkRequest<TStorage>(searchId, offset, size);

            var engine = GetEngine();

            var response = await engine.PerformRequestAsync(request);
            
            ErrorCheck(response);

            return response.FromStorage;
        }

        
    }
}