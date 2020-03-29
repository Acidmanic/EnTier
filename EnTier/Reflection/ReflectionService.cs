



using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;

namespace EnTier.Utility{


    public class ReflectionService :CachedReflection{



        private static ReflectionService instance = null;

        private ReflectionService()
        {
            CacheCurrent();
        }

        public static ReflectionService Make(){

            var obj = new object();

            lock (obj)
            {
                if(instance == null){
                    instance = new ReflectionService();
                }

                instance.ClearFilters();
            }

            return instance;

        }

        
    }
}