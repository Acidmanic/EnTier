using System;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace EnTier.DataAccess.EntityFramework.EventStreamRepositories;

internal class ULongIntEfEsRepository<TEvent>:EntityFrameworkEventStreamRepositoryBase<TEvent,ulong,int>
{
    public ULongIntEfEsRepository(DbSet<EfObjectEntry<ulong, int>> dbSet) : base(dbSet)
    {
    }

    protected override Expression<Func<EfObjectEntry<ulong, int>, bool>> StreamIdEqualityExpression(int streamId)
    {
        return entry => entry.StreamId == streamId;
    }

    protected override Expression<Func<EfObjectEntry<ulong, int>, bool>> EventIdLargerThanExpression(ulong baseEventId)
    {
        return entry => entry.EventId > baseEventId;
    }
}