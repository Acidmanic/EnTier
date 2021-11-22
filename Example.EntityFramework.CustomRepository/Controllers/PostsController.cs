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

        private readonly  IUnitOfWork _unitOfWork;
        
        public PostsController(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }


        [HttpGet]
        [Route("morgh")]
        public IActionResult morgh()
        {
            var repo = _unitOfWork.GetCrudRepository<PostStg, long, DummyRepository>();

            return Ok(repo.GetNoneExistingPosts());
        }
    }
}
