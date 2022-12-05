using EnTier;
using EnTier.Controllers;
using EnTier.UnitOfWork;
using ExampleEntityFramework.DomainModels;
using ExampleEntityFramework.StoragesModels;
using ExampleEntityFramework.TransferModels;
using Microsoft.AspNetCore.Mvc;

namespace ExampleEntityFramework.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PostsController : CrudControllerBase<PostDto,Post,PostStg,long>
    {

        public PostsController(EnTierEssence essence) : base(essence)
        {
        }

        [HttpGet]
        [Route("custom")]
        public IActionResult Custom()
        {
            var repo = UnitOfWork.GetCrudRepository<PostStg, long>() as DummyRepository;

            return Ok(repo?.GetNoneExistingPosts());
        }

    }
}
