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
    [Route("users")]
    public class UsersController : CrudControllerBase<User,long>
    {
        public UsersController(EnTierEssence essence) : base(essence)
        {
            
        }
    }
}
