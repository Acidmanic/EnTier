using System;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace EnTier.DataAccess.EntityFramework.EventStreamRepositories;

internal class LongGuidEfEsRepository<TEvent>:EntityFrameworkEventStreamRepositoryBase<TEvent,long,Guid>
{
    public LongGuidEfEsRepository(DbSet<EfObjectEntry<long, Guid>> dbSet) : base(dbSet)
    {
    }

    protected override Expression<Func<EfObjectEntry<long, Guid>, bool>> StreamIdEqualityExpression(Guid streamId)
    {
        return entry => entry.StreamId == streamId;
    }

    protected override Expression<Func<EfObjectEntry<long, Guid>, bool>> EventIdLargerThanExpression(long baseEventId)
    {
        return entry => entry.EventId > baseEventId;
    }
}