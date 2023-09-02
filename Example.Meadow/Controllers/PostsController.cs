using EnTier;
using EnTier.Attributes;
using EnTier.Controllers;
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

        }
    }
}