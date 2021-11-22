using System;
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

        public TCustomCrudRepository GetCrudRepository<TStorage, TId, TCustomCrudRepository>()
            where TStorage : class, new()
            where TCustomCrudRepository:ICrudRepository<TStorage,TId>
        {
            var dbSet = _context.Set<TStorage>();

            var repoType = typeof(TCustomCrudRepository);

            var repository  = repoType.GetConstructor(new Type[] {typeof(DbSet<TStorage>)})
                .Invoke(new object[]{dbSet});
            
            return (TCustomCrudRepository) repository;
        }

        public void Complete()
        {
            _context.SaveChanges();
        }
    }
}