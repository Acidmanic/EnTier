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
        public PostsController(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public override IActionResult GetAll()
        {
            return base.GetAll();
        }
    }
}