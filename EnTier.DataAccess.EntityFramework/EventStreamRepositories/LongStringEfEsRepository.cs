using System;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace EnTier.DataAccess.EntityFramework.EventStreamRepositories;

internal class LongStringEfEsRepository<TEvent>:EntityFrameworkEventStreamRepositoryBase<TEvent,long,string>
{
    public LongStringEfEsRepository(DbSet<EfObjectEntry<long, string>> dbSet) : base(dbSet)
    {
    }

    protected override Expression<Func<EfObjectEntry<long, string>, bool>> StreamIdEqualityExpression(string streamId)
    {
        return entry => entry.StreamId == streamId;
    }

    protected override Expression<Func<EfObjectEntry<long, string>, bool>> EventIdLargerThanExpression(long baseEventId)
    {
        return entry => entry.EventId > baseEventId;
    }
}