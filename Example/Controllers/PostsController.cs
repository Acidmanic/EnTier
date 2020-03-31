


using System;
using EnTier.Configuration;
using DataAccess;
using Microsoft.AspNetCore.Mvc;
using EnTier.Binding;
using EnTier.Utility;
using EnTier.Controllers;
using EnTier.DataAccess;
using EnTier.Annotations;
using EnTier.Binding.Abstractions;

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