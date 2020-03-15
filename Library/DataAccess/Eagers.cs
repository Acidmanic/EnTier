


using System;
using System.Linq;

namespace DataAccess{


    [System.AttributeUsage(System.AttributeTargets.Class
    , Inherited = false, AllowMultiple = false)]
    sealed class Eager : Attribute
    {

        public Type EntityType{get;private set;}
        public string[] PropertyNames {get; private set;}
        public Eager(Type entityType, params string[] propertyNames)
        {
            EntityType = entityType;

            PropertyNames = propertyNames;
        }



        
    }
}