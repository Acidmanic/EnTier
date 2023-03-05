using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using EnTier.Repositories;
using EnTier.Repositories.Models;
using Microsoft.EntityFrameworkCore;

namespace EnTier.DataAccess.EntityFramework.EventStreamRepositories;

internal abstract class EntityFrameworkEventStreamRepositoryBase<TEvent,TEventId,TStreamId>:EventStreamRepositoryBase<TEvent,TEventId,TStreamId>
{
    public EntityFrameworkEventStreamRepositoryBase(DbSet<EfObjectEntry<TEventId, TStreamId>> dbSet,EnTierEssence essence)
    {
        DbSet = dbSet;
    }

    protected DbSet<EfObjectEntry<TEventId, TStreamId>> DbSet { get; }
    
    
    protected override TEventId AppendEntry(ObjectEntry<TEventId, TStreamId> entry)
    {
        var addedEntry = DbSet.Add(entry.ToEf());

        if (addedEntry.State == EntityState.Added)
        {
            return addedEntry.Entity.EventId;
        }

        return default;
    }

    protected override async Task<IEnumerable<ObjectEntry<TEventId, TStreamId>>> ReadAllEntries()
    {
        var result = await DbSet.ToListAsync();

        return result;
    }

   
    protected abstract Expression<Func<EfObjectEntry<TEventId, TStreamId>, bool>>
        StreamIdEqualityExpression(TStreamId streamId);
    
    protected abstract Expression<Func<EfObjectEntry<TEventId, TStreamId>, bool>>
        EventIdLargerThanExpression(TEventId baseEventId); 
    
    protected override async Task<IEnumerable<ObjectEntry<TEventId, TStreamId>>> ReadEntriesByStreamId(TStreamId streamId)
    {
        var entries = await DbSet
            .Where(StreamIdEqualityExpression(streamId))
            .ToListAsync();

        return entries;
    }

    protected override async Task<IEnumerable<ObjectEntry<TEventId, TStreamId>>> 
        ReadEntryChunk(TStreamId streamId, TEventId baseEventId, long count)
    {
        var entries = await DbSet
            .Where(StreamIdEqualityExpression(streamId))
            .Where(EventIdLargerThanExpression(baseEventId))
            .Take((int)count)
            .ToListAsync();

        return entries;
    }

    protected override async Task<IEnumerable<ObjectEntry<TEventId, TStreamId>>> ReadEntryChunk(TEventId baseEventId, long count)
    {
        var entries = await DbSet
            .Where(EventIdLargerThanExpression(baseEventId))
            .Take((int)count)
            .ToListAsync();

        return entries;
    }
}