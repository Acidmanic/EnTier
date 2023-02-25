using System;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace EnTier.DataAccess.EntityFramework.EventStreamRepositories;

internal class IntIntEfEsRepository<TEvent>:EntityFrameworkEventStreamRepositoryBase<TEvent,int,int>
{
    public IntIntEfEsRepository(DbSet<EfObjectEntry<int, int>> dbSet) : base(dbSet)
    {
    }

    protected override Expression<Func<EfObjectEntry<int, int>, bool>> StreamIdEqualityExpression(int streamId)
    {
        return entry => entry.StreamId == streamId;
    }

    protected override Expression<Func<EfObjectEntry<int, int>, bool>> EventIdLargerThanExpression(int baseEventId)
    {
        return entry => entry.EventId > baseEventId;
    }
}