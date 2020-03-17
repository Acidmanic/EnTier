

using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DataAccess{




    internal class EagerAttributeProcessor{


        public EagerScopeManager MarkEagers<StorageEntity>(MethodBase methodInfo)
        where StorageEntity:class
        {
            var eagers  = Utility.Reflection.GetAttributes<Eager>(methodInfo);
            
            return MarkEagers<StorageEntity>(eagers);
        }
        
        public EagerScopeManager MarkEagers<StorageEntity>(object obj)
        where StorageEntity:class
        {
            var eagers = Utility.Reflection.GetTypeAttributes<Eager>(obj);

            return MarkEagers<StorageEntity>(eagers);

        }


        private EagerScopeManager MarkEagersList<StorageEntity>(List<Eager> eagers)
        where StorageEntity:class
        {
            if(eagers.Count>0){
                
                var scope = new EagerScopeManager();

                var type = typeof(StorageEntity);

                foreach(var eager in eagers){
                    
                    if (eager.EntityType == type){

                        foreach(string prop in eager.PropertyNames){

                            scope.Mark<StorageEntity>( q => q.Include(prop));
                        }
                    }
                }

                return scope;
            }

            return null;
        }

    }
}