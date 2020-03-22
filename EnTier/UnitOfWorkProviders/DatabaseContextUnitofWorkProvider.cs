




using System;
using Microsoft.EntityFrameworkCore;
using Repository;
using Utility;

namespace Providers{



    public class DatabaseContextUnitofWorkProvider<StorageEntity> 
    : UnitOfWorkProviderBase,IEnTierInternal
    where StorageEntity: class
    {


        //TODO: Replace with smart way, or atleast get from Application
        private DbContext GetContext()
        {
            return ReflectionService.Make()
                        .FindConstructor<DbContext>
                        (t => ContainsPropertyOfType<DbSet<StorageEntity>>(t))
                        .Construct();
        }


        private bool ContainsPropertyOfType<T>(Type t)
        {
            var properties = t.GetProperties();

            var findingType = typeof(T);

            foreach(var prop in properties){
                if (prop.CanRead 
                    && prop.CanWrite 
                    && ReflectionService.Make().Extends(prop.PropertyType,findingType)
                ){
                    return true;
                }

            }

            return false;
        }

        
        public override IUnitOfWork Create()
        {
            DbContext context = GetContext();
                
            if (context == null){
                throw new Exception(
                    String.Format("Unable To Find DbContext Containing DbSet<{0}>.",
                    typeof(StorageEntity).FullName)
                );
            }
                    
            return new DatabaseContextGenericUnitOfDataAccess(context);
        }
    }
}