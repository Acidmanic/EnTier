




using Repository;

namespace Providers
{


    public class UnitOfWorkProvider<StorageEntity> : UnitOfWorkProviderBase
    where StorageEntity:class
    {
        public override IUnitOfWork Create()
        {

            var ret = FindUnitOfWork();

            if (ret != null) return ret;

            // TODO: This will be replaced with a factory maybe
            if (EnTierApplication.IsContextBased){
                return new DatabaseContextUnitofWorkProvider<StorageEntity>().Create();
            }

            
            return new NoneContexedGenericUnitOfWorkProvider().Create();

            /*
                Can Find Unit of work? If yes FINISH

                Is System DBContext based? [YES]
                    Make DatabaseContextGenericUnitOfWork
                Is System DBContext based? [NO]
                    Make NoneContexedGenericUnitOfWork

            */
        }
    }
}