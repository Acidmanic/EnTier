using EnTier;
using EnTier.Controllers;
using Example.EventSourcing.EntityFramework.DomainModels;
using Example.EventSourcing.EntityFramework.StoragesModels;
using Example.EventSourcing.EntityFramework.TransferModels;
using Microsoft.AspNetCore.Mvc;

namespace Example.EventSourcing.EntityFramework.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PostsController : CrudControllerBase<PostDto,Post,PostStg,long>
    {
        public PostsController(EnTierEssence essence) : base(essence)
        {
        }
    }
}
