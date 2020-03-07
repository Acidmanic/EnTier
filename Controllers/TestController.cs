




using AutoMapper;
using DataAccess;
using DataTransferModels;
using Microsoft.AspNetCore.Mvc;

namespace Controllers
{


    [Route("api/v1/{Controller}")]
    public class TestController : EntityConterollerBase<StorageModels.User, User, long>
    {

        
        public TestController(IMapper mapper, IProvider<GenericDatabaseUnit> dbProvider) : base(mapper, dbProvider)
        {
        }


        protected override void Configure(ControllerConfigurationBuilder builder){
            builder.ReadonlyImplementation();
        }
    }
}