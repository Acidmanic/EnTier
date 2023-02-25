using System;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace EnTier.DataAccess.EntityFramework.EventStreamRepositories;

internal class IntStringEfEsRepository<TEvent>:EntityFrameworkEventStreamRepositoryBase<TEvent,int,string>
{
    public IntStringEfEsRepository(DbSet<EfObjectEntry<int, string>> dbSet) : base(dbSet)
    {
    }

    protected override Expression<Func<EfObjectEntry<int, string>, bool>> StreamIdEqualityExpression(string streamId)
    {
        return entry => entry.StreamId == streamId;
    }

    protected override Expression<Func<EfObjectEntry<int, string>, bool>> EventIdLargerThanExpression(int baseEventId)
    {
        return entry => entry.EventId > baseEventId;
    }
}