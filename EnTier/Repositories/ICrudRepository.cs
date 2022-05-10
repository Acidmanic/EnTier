using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace EnTier.Repositories
{
    public interface ICrudRepository<TStorage,TId>
    where TStorage:class,new()
    {


        IEnumerable<TStorage> All();

        TStorage Add(TStorage value);
        
        TStorage AddStripped(TStorage value);

        TStorage GetById(TId id);

        IEnumerable<TStorage> Find(Expression<Func<TStorage,bool>> predicate);

        bool Remove(TStorage value);

        bool Remove(TId id);
    }
}