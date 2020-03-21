



using Utility;

namespace Repository{

    public class NoneContexedGenericUnitOfWork : IUnitOfWork, IEnTierGeneric
    {
        public void Compelete()
        {        }

        public void Dispose()
        {        }

        public IRepository<StorageEntity, Tid> GetRepository<StorageEntity, Tid>() where StorageEntity : class
        {
            var ret = ReflectionService.Make().GetCreatorForTypeWhichImplements
                <IRepository<StorageEntity,Tid>>().Construct();

                if(ret == null){
                    //TODO: Later Add Generic None Db Implementations if reasonable
                }

            return ret;
        }
    }
}