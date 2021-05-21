using EnTier.Repositories;
using EnTier.UnitOfWork;
using Microsoft.EntityFrameworkCore;

namespace EnTier.DataAccess.EntityFramework
{
    public class EntityFrameworkUnitOfWork : IUnitOfWork
    {
        private readonly DbContext _context;

        public EntityFrameworkUnitOfWork(DbContext context)
        {
            _context = context;
        }


        public ICrudRepository<TStorage, TId> GetCrudRepository<TStorage, TId>() where TStorage : class, new()
        {
            var dbSet = _context.Set<TStorage>();

            return new EntityFrameWorkCrudRepository<TStorage, TId>(dbSet);
        }

        public void Complete()
        {
            _context.SaveChanges();
        }
    }
}