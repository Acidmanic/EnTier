using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Configuration;
using Controllers;
using Mapping;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Models;
using Plugging;
using Utility;

namespace Example2.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class UsersController : EnTierControllerBase<UserInfo, UserInfo, UserInfo, long>
    {
        public UsersController(IProvider<EnTierConfigurations> configurationProvider) 
            : base(new ObjectMapper(), configurationProvider)
        {
        }
    }
}
