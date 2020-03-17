


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


    public override bool Match(object obj){

        System.Console.WriteLine("**********************");
        System.Console.WriteLine("Iv'e been asked to match: " + obj.ToString());
        System.Console.WriteLine("**********************");

        return base.Match(obj);
    }        

        
    }
}