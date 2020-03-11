




using Microsoft.EntityFrameworkCore;

namespace Repository{

    public class GenericRepository<StorageEntity> : RepositoryBase<StorageEntity> 
        where StorageEntity:class
    {
        public GenericRepository(DbSet<StorageEntity> dbset) : base(dbset)
        {
        }
    }

}

