using EnTier.Repositories;

namespace EnTier.UnitOfWork
{
    public interface IUnitOfWork
    {
        ICrudRepository<TStorage, TId> GetCrudRepository<TStorage, TId>()
            where TStorage : class, new();

        void Complete();
    }
}