using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EnTier.Controllers;
using ExampleInMemorySingleLayerEntity.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ExampleInMemorySingleLayerEntity.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PostsController : CrudControllerBase<Post,string>
    {
        
    }
}
