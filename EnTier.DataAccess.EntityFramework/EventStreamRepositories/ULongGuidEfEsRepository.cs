using System;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace EnTier.DataAccess.EntityFramework.EventStreamRepositories;

internal class ULongGuidEfEsRepository<TEvent>:EntityFrameworkEventStreamRepositoryBase<TEvent,ulong,Guid>
{
    public ULongGuidEfEsRepository(DbSet<EfObjectEntry<ulong, Guid>> dbSet,EnTierEssence essence) : base(dbSet,essence)
    {
    }

    protected override Expression<Func<EfObjectEntry<ulong, Guid>, bool>> StreamIdEqualityExpression(Guid streamId)
    {
        return entry => entry.StreamId == streamId;
    }

    protected override Expression<Func<EfObjectEntry<ulong, Guid>, bool>> EventIdLargerThanExpression(ulong baseEventId)
    {
        return entry => entry.EventId > baseEventId;
    }
}