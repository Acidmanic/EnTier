




using Microsoft.EntityFrameworkCore;
using Repository;
using Utility;

internal class DatabaseContextGenericUnitOfDataAccess 
    : DatabaseContextUnitOfWorkBase, IEnTierGeneric
{


    public DatabaseContextGenericUnitOfDataAccess(DbContext context) : base(context)
    {
    }
    

    public override IRepository<StorageEntity,Tid> GetRepository<StorageEntity,Tid>()
    {

        var ret = base.GetRepository<StorageEntity,Tid>();

        if (ret == default){

            var dbSet = FindDbSet<StorageEntity>();

            if(dbSet != null){
                var constructor = ReflectionService.Make()
                    .FindConstructor<IRepository<StorageEntity,Tid>>(dbSet);
                if(! constructor.IsNull){
                    return constructor.Construct();
                }
                return new GenericRepository<StorageEntity,Tid>(dbSet);
            }
        }

        //TODO: NullRepository Please
        return null;
    }


    private DbSet<StorageEntity> FindDbSet<StorageEntity>()
    where StorageEntity : class
    {

        var reflection = new CachedReflection().CachePropertiesOf(Context);

        var creator =  reflection.GetCreatorForTypeWhichExtends<DbSet<StorageEntity>>();

        return creator.Construct();

    }
}