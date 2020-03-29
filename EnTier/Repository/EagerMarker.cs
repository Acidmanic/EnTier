
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;


namespace EnTier.Repository
{
    public interface IEagerMarker<Entity> where Entity : class
    {

        IEagerMarker<Entity> Include<TProperty>(System.Linq.Expressions.Expression<Func<Entity, TProperty>> expression);
    }
    
    class IQuariableWrapperEagerMarker<Entity> : IEagerMarker<Entity> where Entity : class
    {

        public IQueryable<Entity> Result { get; private set; }

        public IQuariableWrapperEagerMarker(IQueryable<Entity> queryable)
        {
            Result = queryable;
        }
        public IEagerMarker<Entity> Include<TProperty>(System.Linq.Expressions.Expression<Func<Entity, TProperty>> expression)
        {
            Result = Result.Include<Entity, TProperty>(expression);
            
            return this;
        }
    }

    class NoneEagerMarker<Entity> : IEagerMarker<Entity> where Entity : class
    {
        public IEagerMarker<Entity> Include<TProperty>(System.Linq.Expressions.Expression<Func<Entity, TProperty>> expression)
        {
            return this;
        }
    }

}