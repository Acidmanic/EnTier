




using Microsoft.EntityFrameworkCore;

namespace Repository{

    public class GenericRepository<StorageEntity,Tid> 
        : RepositoryBase<StorageEntity,Tid> 
        , IEnTierGeneric
        where StorageEntity:class
    {
        public GenericRepository(DbSet<StorageEntity> dbset) : base(dbset)
        {
        }
    }

}

