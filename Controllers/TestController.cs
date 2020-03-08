




using AutoMapper;
using DataAccess;
using DataTransferModels;
using Microsoft.AspNetCore.Mvc;

namespace Controllers
{


    [Route("api/v1/{Controller}")]
    public class TestController : EntityControllerBase<StorageModels.User, User, long>
    {
        public TestController(IObjectMapper mapper, IProvider<GenericDatabaseUnit> dbProvider) : base(mapper, dbProvider)
        {
        }
    }
}