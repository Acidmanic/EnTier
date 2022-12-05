using EnTier;
using EnTier.Controllers;
using Microsoft.AspNetCore.Mvc;
using ServiceManipulationExample.Models;

namespace ServiceManipulationExample.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PostsController : CrudControllerBase<Post,long>
    {
        public PostsController(EnTierEssence essence) : base(essence)
        {
        }
    }
}
