



using System;
using Microsoft.EntityFrameworkCore;
using Repository;
using Utility;
using Plugging;


namespace Service{


    internal class GenericService<StorageEntity,DomainEntity, Tid> :
        ServiceBase<StorageEntity,DomainEntity, Tid>
        where StorageEntity : class
    {
        public GenericService(IObjectMapper mapper) : base(mapper, new UnitOfDataAccessProvider())
        {
        }


        private class UnitOfDataAccessProvider : IProvider<UnitOfDataAccessBase>
        {

            public UnitOfDataAccessBase Create()
            {
                var reflection = ReflectionService.Make();

                DbContext context = GetContext();
                
                if (context == null){
                    throw new Exception(
                        String.Format("Unable To Find DbContext Containing DbSet<{0}>.",
                        typeof(StorageEntity).FullName)
                    );
                }

                var constructor = reflection.FindConstructor<UnitOfDataAccessBase>(context);

                if(!constructor.IsNull){

                    var ret = constructor.Construct();

                    if( ret != null){
                        return ret;
                    }
                }

                return new GenericUnitOfDataAccess(context);
            }

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
        }

    }
}