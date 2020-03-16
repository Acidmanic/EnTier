




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
    public class TestController : GenericControllerBase
        <StorageModels.User, DomainModels.User, DataTransferModels.User>
    {


        

        public TestController(IObjectMapper mapper, IProvider<EnTierConfigurations> configurationProvider ) : base(mapper)
        {
            EnTierConfigurations = configurationProvider.Create();
        }


        public override IActionResult GetById(long id){
            IActionResult ret;

            using(var scope = new EagerScopeManager()){
                
                scope.Mark<StorageModels.User>();
                // scope.Mark<StorageModels.User>( q => q.Include(u => u.Posts));

                ret = base.GetById(id);

            }

            return ret;
        }
    }
}