using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace EnTier.Repositories
{
    /// <summary>
    /// This class has implemented async methods by forwarding corresponding sync methods in a task.
    /// </summary>
    /// <typeparam name="TStorage"></typeparam>
    /// <typeparam name="TId"></typeparam>
    public abstract class CrudRepositoryForwardAsyncBase<TStorage, TId> : CrudRepositoryBase<TStorage, TId>
        where TStorage : class, new()
    {

        public override Task<IEnumerable<TStorage>> AllAsync(bool readFullTree = false)
        {
            return Task.Run(() => All(readFullTree));
        }

        public override Task<TStorage> UpdateAsync(TStorage value)
        {
            return Task.Run(() => Update(value));
        }

        protected override Task<TStorage> InsertAsync(TStorage value)
        {
            return Task.Run(() => Insert(value));
        }

        public override Task<TStorage> SetAsync(TStorage value)
        {
            return Task.Run(() => Set(value));
        }

        public override Task<TStorage> GetByIdAsync(TId id,bool readFullTree = false)
        {
            return Task.Run(() => GetById(id,readFullTree));
        }

        public override Task<IEnumerable<TStorage>> FindAsync(Expression<Func<TStorage, bool>> predicate)
        {
            return Task.Run(() => Find(predicate));
        }

        public override Task<bool> RemoveAsync(TStorage value)
        {
            return Task.Run(() => Remove(value));
        }

        public override Task<bool> RemoveAsync(TId id)
        {
            return Task.Run(() => Remove(id));
        }

        
    }
}