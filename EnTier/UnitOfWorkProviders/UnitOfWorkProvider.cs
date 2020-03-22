




using Repository;

namespace Providers
{


    public class UnitOfWorkProvider<StorageEntity> : UnitOfWorkProviderBase
    where StorageEntity:class
    {
        public override IUnitOfWork Create()
        {

            // TODO: This will be replaced with a factory maybe
            if (EnTierApplication.IsContextBased){
                return new DatabaseContextUnitofWorkProvider<StorageEntity>().Create();
            }

            
            return new NoneContexedGenericUnitOfWorkProvider().Create();

        }
    }
}