


using System;
using EnTier.Context;

namespace EnTier.Controllers
{

    public class ControllerConfigurations{

        public bool ImplementsGetAll{get;set;}

        public bool ImplementsGetById{get;set;}

        public bool ImplementsCreateNew{get;set;}

        public bool ImplementsUpdate{get;set;}

        public bool ImplementsDeleteById{get;set;}

        public bool ImplementsDeleteByEntity{get;set;}
        
    }

}