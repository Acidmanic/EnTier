



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
                var ret = FindUnitOfWork();

                DbContext context = null;

                if(ret == null){
                    
                    context = GetContext();
                
                    if (context == null){
                        throw new Exception(
                            String.Format("Unable To Find DbContext Containing DbSet<{0}>.",
                            typeof(StorageEntity).FullName)
                        );
                    }
                    
                    ret = FindUnitOfWork(context);
                }

                return new GenericUnitOfDataAccess(context);
            }


            public UnitOfDataAccessBase FindUnitOfWork(params object[] args){

                var reflection = ReflectionService.Make();

                var constructor = reflection.FindConstructor<UnitOfDataAccessBase>(args);

                if(!constructor.IsNull){

                    var ret =  constructor.Construct();

                    return ret;
                }

                return null;
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