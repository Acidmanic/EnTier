using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EnTier.Controllers;
using EnTier.Regulation;
using ExampleRegulation.Contracts;
using ExampleRegulation.Models;
using ExampleRegulation.Regulators;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ExampleRegulation.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PostsController : CrudControllerBase<Post, string>
    {
        public PostsController(IPostRegulator regulator) : base(regulator)
        {
        }
    }
}