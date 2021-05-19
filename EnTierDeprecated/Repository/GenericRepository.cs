




using EnTier;
using EnTier.Context;
using Microsoft.EntityFrameworkCore;

namespace EnTier.Repository{

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

