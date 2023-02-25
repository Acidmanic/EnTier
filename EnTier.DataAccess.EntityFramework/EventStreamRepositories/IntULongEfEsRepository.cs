using System;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace EnTier.DataAccess.EntityFramework.EventStreamRepositories;

internal class IntULongEfEsRepository<TEvent>:EntityFrameworkEventStreamRepositoryBase<TEvent,int,ulong>
{
    public IntULongEfEsRepository(DbSet<EfObjectEntry<int, ulong>> dbSet) : base(dbSet)
    {
    }

    protected override Expression<Func<EfObjectEntry<int, ulong>, bool>> StreamIdEqualityExpression(ulong streamId)
    {
        return entry => entry.StreamId == streamId;
    }

    protected override Expression<Func<EfObjectEntry<int, ulong>, bool>> EventIdLargerThanExpression(int baseEventId)
    {
        return entry => entry.EventId > baseEventId;
    }
}