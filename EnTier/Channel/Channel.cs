



using Controllers;
using Microsoft.AspNetCore.Mvc;
using Repository;
using Service;

namespace Channel{





    public class Channel{

        public ControllerBase Controller{get; private set;}

        public IService Service{get; private set;}

        public IUnitOfWork UnitOfWork {get;private set;}

        public IRepository Repository{get;set;}


    }
}