using EnTier;
using EnTier.Attributes;
using EnTier.Controllers;
using EnTier.DataAccess.Meadow.GenericCrudRequests;
using Example.Meadow.Models;
using Microsoft.AspNetCore.Mvc;

namespace Example.Meadow.Controllers
{
    [ApiController]
    [Route("posts")]
    [FullTreeRead]
    public class PostsController : CrudControllerBase<Post, long>
    {
        public PostsController(EnTierEssence essence) : base(essence)
        {

            var d = new ReadByIdStorageRequest<Post, long>();
            
            
        }
    }
}