




using Microsoft.EntityFrameworkCore;
using Repository;
using Utility;

internal class GenericUnitOfDataAccess : UnitOfDataAccessBase
{


    public GenericUnitOfDataAccess(DbContext context) : base(context)
    {
    }

    public override IRepository<StorageEntity> GetRepository<StorageEntity>()
    {

        var ret = base.GetRepository<StorageEntity>();

        if (ret == default){

            var dbSet = FindDbSet<StorageEntity>();

            if(dbSet != null){
                var constructor = ReflectionService.Make()
                    .FindConstructor<IRepository<StorageEntity>>(dbSet);
                if(! constructor.IsNull){
                    return constructor.Construct();
                }
                return new GenericRepository<StorageEntity>(dbSet);
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