


using System;
using Configuration;
using DataAccess;
using Microsoft.AspNetCore.Mvc;
using Plugging;
using Utility;

namespace Controllers{


    [Route("api/v1/{Controller}")]
    public class PostsController : EnTierControllerBase<StorageModels.Post, DomainModels.Post, DataTransferModels.Post>
    {
        public PostsController(IObjectMapper mapper, IProvider<EnTierConfigurations> configurationProvider) : base(mapper, configurationProvider)
        {
        }



        [Eager(typeof(StorageModels.Post),nameof(StorageModels.Post.Creator))]
        public override IActionResult GetById(long id){
            return base.GetById(id);
        }
    }
}