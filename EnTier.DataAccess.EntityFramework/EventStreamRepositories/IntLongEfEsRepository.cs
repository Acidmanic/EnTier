using System;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace EnTier.DataAccess.EntityFramework.EventStreamRepositories;

internal class IntLongEfEsRepository<TEvent>:EntityFrameworkEventStreamRepositoryBase<TEvent,int,long>
{
    public IntLongEfEsRepository(DbSet<EfObjectEntry<int, long>> dbSet,EnTierEssence essence) : base(dbSet,essence)
    {
    }

    protected override Expression<Func<EfObjectEntry<int, long>, bool>> StreamIdEqualityExpression(long streamId)
    {
        return entry => entry.StreamId == streamId;
    }

    protected override Expression<Func<EfObjectEntry<int, long>, bool>> EventIdLargerThanExpression(int baseEventId)
    {
        return entry => entry.EventId > baseEventId;
    }
}