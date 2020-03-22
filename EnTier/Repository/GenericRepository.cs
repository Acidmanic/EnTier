




using Context;
using Microsoft.EntityFrameworkCore;

namespace Repository{

    internal class GenericRepository<StorageEntity,Tid> 
        : RepositoryBase<StorageEntity,Tid> 
        , IEnTierGeneric
        where StorageEntity:class
    {
        public GenericRepository(IDataset<StorageEntity> dbset) : base(dbset)
        {
        }
    }

}

