


using System;
using System.Linq;

namespace DataAccess{


    [System.AttributeUsage(System.AttributeTargets.Class | AttributeTargets.Method
    , Inherited = false, AllowMultiple = false)]
    public sealed class Eager : Attribute
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