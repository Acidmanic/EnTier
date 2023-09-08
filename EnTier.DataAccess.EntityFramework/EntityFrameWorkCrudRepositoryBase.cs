using System.Diagnostics.CodeAnalysis;
using EnTier.DataAccess.EntityFramework.FullTreeHandling;
using EnTier.DataAccess.JsonFile;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace EnTier.DataAccess.EntityFramework;

public class EntityFrameWorkCrudRepositoryBase<TStorage, TId> : EntityFrameWorkCrudRepository<TStorage, TId>
    where TStorage : class, new()
{
    /// <summary>
    /// Just to bundle parameters passing to base constructor
    /// </summary>
    private class BaseParams
    {
        public DbSet<TStorage> DbSet { get; set; }
        public DbSet<MarkedSearchIndex<TStorage, TId>> Index { get; set; }
        public DbSet<MarkedFilterResult<TStorage, TId>> Filter { get; set; }
        public IFullTreeMarker<TStorage> Marker { get; set; }
    }


    public EntityFrameWorkCrudRepositoryBase(DbContext dbContext, [AllowNull] IFullTreeMarker<TStorage> fullTreeMarker)
        : this(Initialize(dbContext, fullTreeMarker))
    {
    }

    public EntityFrameWorkCrudRepositoryBase(DbContext dbContext)
        : this(Initialize(dbContext, null))
    {
    }

    private EntityFrameWorkCrudRepositoryBase(BaseParams p) : base(p.DbSet, p.Filter, p.Index, p.Marker)
    {
    }

    private static BaseParams Initialize(DbContext context, IFullTreeMarker<TStorage> fullTreeMarker)
    {
        return new BaseParams
        {
            DbSet = context.Set<TStorage>(),
            Filter = context.Set<MarkedFilterResult<TStorage, TId>>(),
            Index = context.Set<MarkedSearchIndex<TStorage, TId>>(),
            Marker = fullTreeMarker ?? new NullFullTreeMarker<TStorage>()
        };
    }
}