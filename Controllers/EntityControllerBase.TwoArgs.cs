





using AutoMapper;
using DataAccess;

namespace Controllers{


    public abstract class EntityControllerBase<StorageModel, TransferModel> 
        : EntityControllerBase<StorageModel, TransferModel, long> 
        where StorageModel:class 
    {
        public EntityControllerBase(IObjectMapper mapper, IProvider<GenericDatabaseUnit> dbProvider) : base(mapper, dbProvider)
        {
        }
    }
}