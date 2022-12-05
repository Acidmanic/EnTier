using System;
using EnTier;
using EnTier.Controllers;
using EnTier.Mapper;
using EnTier.UnitOfWork;
using Example.AutoMapper.Domain;
using Example.AutoMapper.Dto;
using Example.AutoMapper.Storage;
using Microsoft.AspNetCore.Mvc;

namespace Example.AutoMapper.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PostsController : CrudControllerBase<PostDto,Post,PostStg,string,Guid,Guid>
    {
        public PostsController(EnTierEssence essence) : base(essence)
        {
            
        }
    }
}
