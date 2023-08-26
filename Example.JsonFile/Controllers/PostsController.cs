using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EnTier;
using EnTier.Controllers;
using EnTier.UnitOfWork;
using ExampleJsonFile.Domain;
using ExampleJsonFile.Dto;
using ExampleJsonFile.Storage;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ExampleJsonFile.Controllers
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
