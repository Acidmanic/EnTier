using EnTier;
using EnTier.AutoWrap;
using EnTier.Controllers;
using EnTier.Mapper;
using EnTier.UnitOfWork;
using Example.Meadow.Models;
using Microsoft.AspNetCore.Mvc;

namespace Example.Meadow.Controllers
{
    [ApiController]
    [Route("posts")]
    public class PostsController : CrudControllerBase<Post, long>
    {
        public PostsController(EnTierEssence essence) : base(essence)
        {
        }
    }
}