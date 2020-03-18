




using DataAccess;
using DataTransferModels;
using Microsoft.AspNetCore.Mvc;
using Service;
using Plugging;
using Microsoft.EntityFrameworkCore;
using Utility;
using Configuration;

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
    }
}