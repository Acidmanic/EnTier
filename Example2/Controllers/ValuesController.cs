using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Controllers;
using Microsoft.AspNetCore.Mvc;
using Models;
using Plugging;

namespace Example2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : EnTierControllerBase<UserInfo,UserInfo,UserInfo,long>
    {
        public UsersController(IObjectMapper mapper) : base(mapper)
        {
        }

    }
}
