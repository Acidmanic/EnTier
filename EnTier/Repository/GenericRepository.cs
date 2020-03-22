




using Microsoft.EntityFrameworkCore;

namespace Repository{

    internal class GenericDatabaseContextRepository<StorageEntity,Tid> 
        : DatabaseContextRepositoryBase<StorageEntity,Tid> 
        , IEnTierGeneric
        where StorageEntity:class
    {
        public GenericDatabaseContextRepository(DbSet<StorageEntity> dbset) : base(dbset)
        {
        }
    }

}

