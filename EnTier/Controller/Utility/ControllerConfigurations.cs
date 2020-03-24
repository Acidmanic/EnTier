


using System;
using Context;

namespace Controllers{

    public class ControllerConfigurations{

        public bool ImplementsGetAll{get;set;}

        public bool ImplementsGetById{get;set;}

        public bool ImplementsCreateNew{get;set;}

        public bool ImplementsUpdate{get;set;}

        public bool ImplementsDeleteById{get;set;}

        public bool ImplementsDeleteByEntity{get;set;}

        public Type ContextType {get;set;} = typeof(NullContext);

        public bool UseConfiguredContextType {get;set;} = false;
        

    }

}