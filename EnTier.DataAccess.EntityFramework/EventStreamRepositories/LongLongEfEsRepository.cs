using System;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace EnTier.DataAccess.EntityFramework.EventStreamRepositories;

internal class LongLongEfEsRepository<TEvent>:EntityFrameworkEventStreamRepositoryBase<TEvent,long,long>
{
    public LongLongEfEsRepository(DbSet<EfObjectEntry<long, long>> dbSet) : base(dbSet)
    {
    }

    protected override Expression<Func<EfObjectEntry<long, long>, bool>> StreamIdEqualityExpression(long streamId)
    {
        return entry => entry.StreamId == streamId;
    }

    protected override Expression<Func<EfObjectEntry<long, long>, bool>> EventIdLargerThanExpression(long baseEventId)
    {
        return entry => entry.EventId > baseEventId;
    }
}