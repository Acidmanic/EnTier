




using DataAccess;
using DataTransferModels;
using Microsoft.AspNetCore.Mvc;
using Service;
using Plugging;
using Microsoft.EntityFrameworkCore;

namespace Controllers
{


    [Route("api/v1/{Controller}")]
    public class TestController : EntityControllerBase
        <StorageModels.User, DomainModels.User, DataTransferModels.User>
    {


        

        public TestController(IObjectMapper mapper) : base(mapper)
        {
            
        }


        public override IActionResult GetAll(){

            IActionResult ret;

            using(var scope = new EagerScopeManager()){
                
                scope.Mark<StorageModels.User>( q => q.Include(u => u.Posts));

                ret = base.GetAll();

            }

            return ret;
        }
    }
}