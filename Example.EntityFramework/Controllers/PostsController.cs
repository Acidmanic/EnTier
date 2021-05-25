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
        public PostsController(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
    }
}
