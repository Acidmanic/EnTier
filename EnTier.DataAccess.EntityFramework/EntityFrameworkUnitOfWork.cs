using System;
using System.Diagnostics;
using Acidmanic.Utilities.Filtering.Models;
using EnTier.DataAccess.EntityFramework.EventStreamRepositories;
using EnTier.DataAccess.EntityFramework.FullTreeHandling;
using EnTier.Repositories;
using EnTier.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EnTier.DataAccess.EntityFramework
{
    public class EntityFrameworkUnitOfWork : UnitOfWorkBase
    {
        private readonly DbContext _context;
        private readonly Func<Type, object> _getDbSetByType;
        private readonly EnTierEssence _essence;

        public EntityFrameworkUnitOfWork(EnTierEssence essence, DbContext context) : base(essence)
        {
            _essence = essence;
            _context = context;

            var contextType = typeof(DbContext);

            var getDbSetMethod = contextType.GetMethod(nameof(DbContext.Set), new Type[] { });

            _getDbSetByType = entityType =>
            {
                var method = getDbSetMethod?.MakeGenericMethod(new Type[] { entityType });

                return method?.Invoke(_context, new object[] { });
            };
        }

        public override IEventStreamRepository<TEvent, TEventId, TStreamId> GetStreamRepository<TEvent, TEventId,
            TStreamId>()
        {
            var dbSet = _context.Set<EfObjectEntry<TEventId, TStreamId>>();

            var repository = EfEventStreamRepositoryFactory.Instance.Make<TEvent, TEventId, TStreamId>(dbSet, Essence);

            if (repository is NullEfEventStreamRepository)
            {
                Logger.LogError(
                    "EntityFrameworkUnitOfWork was not able to instantiate a proper EventStreamRepository " +
                    "for Event Type: {EventType}, EventId Type: {EventId} and StreamId Type: {StreamId}. " +
                    "Please consider that:" +
                    "\nEventId type can only be int | long | ulong." +
                    "\nStreamId type can only be int | long | ulong | string | guid." +
                    "\nYour DbContext must provide DbSet<EfObjectEntry<TEventId,TStreamId>>",
                    typeof(TEvent).Name, typeof(TEventId).Name, typeof(TStreamId).Name);
            }

            return repository;
        }

        protected override ICrudRepository<TStorage, TId> CreateDefaultCrudRepository<TStorage, TId>()
        {
            var dbSet = _context.Set<TStorage>();
            var filterResultSet = _context.Set<FilterResult>();

            var marker = _essence.ResolveOrDefault<IFullTreeMarker<TStorage>>
                (new NullFullTreeMarker<TStorage>());

            return new EntityFrameWorkCrudRepository<TStorage, TId>(dbSet, filterResultSet, marker);
        }


        public override void Complete()
        {
            _context.SaveChanges();
        }

        public override void Dispose()
        {
            _context.Dispose();
        }

        public override IDataBoundRepository GetDataBoundRepository<TStorage>()
        {
            var dbSet = _context.Set<TStorage>();

            return new EntityFrameworkDataBoundRepository<TStorage>(dbSet);
        }
    }
}