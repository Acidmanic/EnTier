



using System;
using Microsoft.EntityFrameworkCore;
using Repository;
using Utility;
using Plugging;
using Providers;

namespace Service{


    internal class GenericService<StorageEntity,DomainEntity, Tid> :
        ServiceBase<StorageEntity,DomainEntity, Tid>
        , IEnTierGeneric
        where StorageEntity : class
    {
        public GenericService() : base(){        }

    }
}