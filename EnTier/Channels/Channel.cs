



using System;
using Controllers;
using Microsoft.AspNetCore.Mvc;
using Repository;
using Service;

namespace Channels{





    public class Channel{

        public Type ControllerType{get; set;}

        public Type ServiceType{get; set;}

        public Type UnitOfWorkType {get; set;}

        public Type RepositoryType{get;set;}


    }
}