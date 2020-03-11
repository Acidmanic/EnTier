



using System;
using Microsoft.EntityFrameworkCore;
using Repository;
using Utility;

namespace Services{


    public class GenericService<StorageEntity,DomainEntity, Tid> :
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

                var constructor = reflection.FindConstructor<UnitOfDataAccessBase>(context);

                if(constructor != null){
                    var ret = constructor();

                    if( ret != null){
                        return ret;
                    }
                }

                return new GenericUnitOfDataAccess(context);
            }

            private DbContext GetContext()
            {
                return ReflectionService.Make()
                            .FindConstructor<DbContext>(t => ContainsBdSetOf<StorageEntity>(t))();
            }

            private bool ContainsBdSetOf<T>(Type t)
            {
                var properties = t.GetProperties();

                var entityType = typeof(T);

                foreach(var prop in properties){
                    if (prop.CanRead 
                        && prop.CanWrite 
                        && ReflectionService.Make().Implements(prop.DeclaringType,entityType)
                    ){
                        return true;
                    }

                }

                return false;
            }
        }

    }
}