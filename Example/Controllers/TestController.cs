




using DataAccess;
using DataTransferModels;
using Microsoft.AspNetCore.Mvc;
using Service;
using EnTier.Binding;
using Microsoft.EntityFrameworkCore;
using EnTier.Utility;
using EnTier.Configuration;
using System;
using EnTier.Controllers;
using EnTier.DataAccess;
using EnTier.Annotations;

namespace Controllers
{


    [Route("api/v1/{Controller}")]
    [Eager(typeof(StorageModels.User),nameof(StorageModels.User.Posts))]
    public class TestController : EnTierControllerBase
        <StorageModels.User, DomainModels.User, DataTransferModels.User>
    {


        

        public TestController(IObjectMapper mapper, IProvider<EnTierConfigurations> configurationProvider ) : base(mapper,configurationProvider)
        {}



        
        public override IActionResult GetAll(){
            return base.GetAll();
        }

        [Eager(typeof(StorageModels.User))]
        [HttpGet]
        [Route("Users")]
        public IActionResult GetUsersNonEager(){
            return base.GetAll();
        }

        [HttpGet]
        [Route("Error")]
        public IActionResult RaiseError(){

            var ret = IO.SafeRun<object>(() =>{
                throw new Exception("This is a testy Error. It's fiered on purpose");
            });

            return IO.Map<object,object>(ret);
        }
    }
}