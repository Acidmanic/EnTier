
using Microsoft.EntityFrameworkCore;
using System.Linq;


namespace DataAccess{




    internal class EagerAttributeProcessor{

        public EagerScopeManager MarkEagers<StorageEntity>(object obj)
        where StorageEntity:class
        {
            var eagers = Utility.Reflection.GetAttributes<Eager>(obj);

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