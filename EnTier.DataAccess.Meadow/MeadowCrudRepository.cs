using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Acidmanic.Utilities.Reflection.Extensions;
using EnTier.DataAccess.Meadow.GenericCrudRequests;
using EnTier.Repositories;
using Meadow;
using Meadow.Configuration;

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


        public override IEnumerable<TStorage> All()
        {
            var request = new ReadAllStorageRequest<TStorage>();

            var engine = GetEngine();

            var response = engine.PerformRequest(request);

            return response.FromStorage;
        }


        protected override TStorage Insert(TStorage value)
        {
            var request = new InsertStorageRequest<TStorage>();

            request.ToStorage = value;

            var engine = GetEngine();

            var response = engine.PerformRequest(request);

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

            var result = response.FromStorage.FirstOrDefault();

            return result != null && result.Success;
        }

        protected MeadowEngine GetEngine()
        {
            var engine = new MeadowEngine(Configuration);

            return engine;
        }

        protected TId TryReadId(TStorage value)
        {
            try
            {
                var type = typeof(TStorage);

                var leaf = type.GetUniqueIdFieldLeaf();

                if (leaf != null)
                {
                    return (TId) leaf.Evaluator.Read(value);
                }
            }
            catch (Exception e)
            {
            }

            return default;
        }
    }
}