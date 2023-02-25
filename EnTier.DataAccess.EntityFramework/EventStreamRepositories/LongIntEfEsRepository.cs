using System;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace EnTier.DataAccess.EntityFramework.EventStreamRepositories;

internal class LongIntEfEsRepository<TEvent>:EntityFrameworkEventStreamRepositoryBase<TEvent,long,int>
{
    public LongIntEfEsRepository(DbSet<EfObjectEntry<long, int>> dbSet) : base(dbSet)
    {
    }

    protected override Expression<Func<EfObjectEntry<long, int>, bool>> StreamIdEqualityExpression(int streamId)
    {
        return entry => entry.StreamId == streamId;
    }

    protected override Expression<Func<EfObjectEntry<long, int>, bool>> EventIdLargerThanExpression(long baseEventId)
    {
        return entry => entry.EventId > baseEventId;
    }
}