
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;


namespace Repository
{
    public interface EagerMarker<Entity> where Entity : class
    {

        EagerMarker<Entity> Include<TProperty>(System.Linq.Expressions.Expression<Func<Entity, TProperty>> expression);
    }
    
    class IQuariableWrapperEagerMarker<Entity> : EagerMarker<Entity> where Entity : class
    {

        public IQueryable<Entity> Result { get; private set; }

        public IQuariableWrapperEagerMarker(IQueryable<Entity> queryable)
        {
            Result = queryable;
        }
        public EagerMarker<Entity> Include<TProperty>(System.Linq.Expressions.Expression<Func<Entity, TProperty>> expression)
        {
            Result = Result.Include<Entity, TProperty>(expression);
            
            return this;
        }
    }

    class NoneEagerMarker<Entity> : EagerMarker<Entity> where Entity : class
    {
        public EagerMarker<Entity> Include<TProperty>(System.Linq.Expressions.Expression<Func<Entity, TProperty>> expression)
        {
            return this;
        }
    }

}