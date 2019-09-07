
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace mock_server.Controllers{

    [Route("api/v1/client/auth")]
    [ApiController]
    public class AuthController:ControllerBase{


        [HttpGet]
        [Route("login")]
        public ActionResult<IEnumerable<string>> Login()
        {
            return new string[] { "value1", "value2" };
        }

        [HttpPost]
        [Route("verify")]
        public ActionResult<IEnumerable<string>> Verify()
        {
            return new string[] { "value1", "value2" };
        }



    }

}