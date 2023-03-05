using System;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace EnTier.DataAccess.EntityFramework.EventStreamRepositories;

internal class LongULongEfEsRepository<TEvent>:EntityFrameworkEventStreamRepositoryBase<TEvent,long,ulong>
{
    public LongULongEfEsRepository(DbSet<EfObjectEntry<long, ulong>> dbSet,EnTierEssence essence) : base(dbSet,essence)
    {
    }

    protected override Expression<Func<EfObjectEntry<long, ulong>, bool>> StreamIdEqualityExpression(ulong streamId)
    {
        return entry => entry.StreamId == streamId;
    }

    protected override Expression<Func<EfObjectEntry<long, ulong>, bool>> EventIdLargerThanExpression(long baseEventId)
    {
        return entry => entry.EventId > baseEventId;
    }
}