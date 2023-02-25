using System;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace EnTier.DataAccess.EntityFramework.EventStreamRepositories;

internal class ULongStringEfEsRepository<TEvent>:EntityFrameworkEventStreamRepositoryBase<TEvent,ulong,string>
{
    public ULongStringEfEsRepository(DbSet<EfObjectEntry<ulong, string>> dbSet) : base(dbSet)
    {
    }

    protected override Expression<Func<EfObjectEntry<ulong, string>, bool>> StreamIdEqualityExpression(string streamId)
    {
        return entry => entry.StreamId == streamId;
    }

    protected override Expression<Func<EfObjectEntry<ulong, string>, bool>> EventIdLargerThanExpression(ulong baseEventId)
    {
        return entry => entry.EventId > baseEventId;
    }
}