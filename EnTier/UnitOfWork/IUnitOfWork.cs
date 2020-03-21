







using System;

namespace Repository
{
    public interface IUnitOfWork:IDisposable{


        IRepository<StorageEntity,Tid> GetRepository<StorageEntity,Tid>() where StorageEntity : class;
        void Compelete();
    }
}