



using System;
using Microsoft.EntityFrameworkCore;
using EnTier.Repository;
using EnTier.Utility;
using EnTier.Plugging;
using EnTier;

namespace EnTier.Service{


    internal class GenericService<StorageEntity,DomainEntity, Tid> :
        ServiceBase<StorageEntity,DomainEntity, Tid>
        , IEnTierGeneric
        where StorageEntity : class
    {
        public GenericService() : base(){        }

    }
}