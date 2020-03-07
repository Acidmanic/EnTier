


using System;
using DataAccess;
using Microsoft.AspNetCore.Mvc;

namespace Controllers{


    [Route("api/v1/{Controller}")]
    public class PostsController : EntityControllerBase<StorageModels.Post, DataTransferModels.Post>
    {
        public PostsController(IObjectMapper mapper, IProvider<GenericDatabaseUnit> dbProvider) : base(mapper, dbProvider)
        {
        }


        public override IActionResult CreateNew(DataTransferModels.Post entity){
            entity.PostDate = DateTime.Now;
            return base.CreateNew(entity);
        }
    }
}