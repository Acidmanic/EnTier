







using System;

namespace EnTier.Repository
{
    public interface IUnitOfWork:IDisposable{


        IRepository<StorageEntity,Tid> GetRepository<StorageEntity,Tid>() where StorageEntity : class;
        void Compelete();
    }
}