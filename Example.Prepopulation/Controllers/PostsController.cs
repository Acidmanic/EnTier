using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EnTier;
using EnTier.Controllers;
using Example.Prepopulation.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Example.Prepopulation.Controllers
{
    [ApiController]
    [Route("posts")]
    public class PostsController : CrudControllerBase<Post,long>
    {
        public PostsController(EnTierEssence essence) : base(essence)
        {
            
        }
    }
}
