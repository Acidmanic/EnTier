using System;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace EnTier.DataAccess.EntityFramework.EventStreamRepositories;

internal class ULongULongEfEsRepository<TEvent> : EntityFrameworkEventStreamRepositoryBase<TEvent, ulong, ulong>
{
    public ULongULongEfEsRepository(DbSet<EfObjectEntry<ulong, ulong>> dbSet, EnTierEssence essence) : base(dbSet,
        essence)
    {
    }

    protected override Expression<Func<EfObjectEntry<ulong, ulong>, bool>> StreamIdEqualityExpression(ulong streamId)
    {
        return entry => entry.StreamId == streamId;
    }

    protected override Expression<Func<EfObjectEntry<ulong, ulong>, bool>> EventIdLargerThanExpression(
        ulong baseEventId)
    {
        return entry => entry.EventId > baseEventId;
    }
}