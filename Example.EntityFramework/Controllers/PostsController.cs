using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EnTier.Controllers;
using EnTier.Services;
using EnTier.UnitOfWork;
using ExampleEntityFramework.DomainModels;
using ExampleEntityFramework.StoragesModels;
using ExampleEntityFramework.TransferModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

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
