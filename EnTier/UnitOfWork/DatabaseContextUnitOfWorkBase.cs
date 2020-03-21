




using System;
using Microsoft.EntityFrameworkCore;
using Utility;

namespace Repository
{



    public abstract class DatabaseContextUnitOfWorkBase : IUnitOfWork
    {

        protected  DbContext Context {get; private set;}

        private readonly CachedReflection _reflection;
        

        public virtual IRepository<StorageEntity,Tid> GetRepository<StorageEntity,Tid>()
        where StorageEntity : class
        {

            var ret = _reflection.GetCreatorForTypeWhichImplements
                <IRepository<StorageEntity,Tid>>().Construct();

            return ret;
        }


        public DatabaseContextUnitOfWorkBase(DbContext context)
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