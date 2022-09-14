﻿using EnTier.Controllers;
using ExampleInMemorySingleLayerEntity.Models;
using Microsoft.AspNetCore.Mvc;

namespace ExampleInMemorySingleLayerEntity.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PostsController : CrudControllerBase<Post,string>
    {
        
    }
}
