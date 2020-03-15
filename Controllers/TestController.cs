




using DataAccess;
using DataTransferModels;
using Microsoft.AspNetCore.Mvc;
using Service;
using Plugging;
using Microsoft.EntityFrameworkCore;

namespace Controllers
{


    [Route("api/v1/{Controller}")]
    // [Eager(typeof(StorageModels.User),nameof(StorageModels.User.Posts))]
    public class TestController : EntityControllerBase
        <StorageModels.User, DomainModels.User, DataTransferModels.User>
    {


        

        public TestController(IObjectMapper mapper) : base(mapper)
        {
            
        }


        public override IActionResult GetById(long id){
            IActionResult ret;

            using(var scope = new EagerScopeManager()){
                
                scope.Mark<StorageModels.User>();
                scope.Mark<StorageModels.User>( q => q.Include(u => u.Posts));

                ret = base.GetById(id);

            }

            return ret;
        }
    }
}