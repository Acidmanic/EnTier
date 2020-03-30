﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EnTier.Configuration;
using EnTier.Controllers;
using Mapping;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Models;
using EnTier.Binding;
using EnTier.Utility;

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
