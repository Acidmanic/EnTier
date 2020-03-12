




using DataAccess;
using DataTransferModels;
using Microsoft.AspNetCore.Mvc;
using Service;
using Plugging;

namespace Controllers
{


    [Route("api/v1/{Controller}")]
    public class TestController : EntityControllerBase
        <StorageModels.User, DomainModels.User, DataTransferModels.User>
    {
        public TestController(IObjectMapper mapper) : base(mapper)
        {
        }
    }
}