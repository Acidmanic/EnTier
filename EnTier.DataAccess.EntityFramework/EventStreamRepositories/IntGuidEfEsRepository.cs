using System;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace EnTier.DataAccess.EntityFramework.EventStreamRepositories;

internal class IntGuidEfEsRepository<TEvent>:EntityFrameworkEventStreamRepositoryBase<TEvent,int,Guid>
{
    public IntGuidEfEsRepository(DbSet<EfObjectEntry<int, Guid>> dbSet) : base(dbSet)
    {
    }

    protected override Expression<Func<EfObjectEntry<int, Guid>, bool>> StreamIdEqualityExpression(Guid streamId)
    {
        return entry => entry.StreamId == streamId;
    }

    protected override Expression<Func<EfObjectEntry<int, Guid>, bool>> EventIdLargerThanExpression(int baseEventId)
    {
        return entry => entry.EventId > baseEventId;
    }
}