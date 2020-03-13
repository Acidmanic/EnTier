




using System;
using Microsoft.EntityFrameworkCore;
using Utility;

namespace Repository
{



    public abstract class UnitOfDataAccessBase : IUnitOfWork
    {

        protected  DbContext Context {get; private set;}

        private readonly CachedReflection _reflection;
        

        public virtual IRepository<StorageEntity> GetRepository<StorageEntity>()
        where StorageEntity : class
        {

            var ret = _reflection.GetCreatorForTypeWhichImplements
                <IRepository<StorageEntity>>().Construct();

            return ret;
        }


        public UnitOfDataAccessBase(DbContext context)
        {
            Context = context;

            _reflection = new CachedReflection();

            _reflection.CachePropertiesOf(this);
        }

        public void Compelete()
        {
            Context.SaveChanges();
        }

        public void Dispose()
        {
            Context.Dispose();
        }
    }
}