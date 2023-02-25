using System;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace EnTier.DataAccess.EntityFramework.EventStreamRepositories;

internal class ULongLongEfEsRepository<TEvent>:EntityFrameworkEventStreamRepositoryBase<TEvent,ulong,long>
{
    public ULongLongEfEsRepository(DbSet<EfObjectEntry<ulong, long>> dbSet) : base(dbSet)
    {
    }

    protected override Expression<Func<EfObjectEntry<ulong, long>, bool>> StreamIdEqualityExpression(long streamId)
    {
        return entry => entry.StreamId == streamId;
    }

    protected override Expression<Func<EfObjectEntry<ulong, long>, bool>> EventIdLargerThanExpression(ulong baseEventId)
    {
        return entry => entry.EventId > baseEventId;
    }
}