
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace mock_server.Controllers{

    [Route("api/v1/client/auth")]
    [ApiController]
    public class AuthController:ControllerBase{



        private ILoginService _loginService;
        private IMapper _mapper;



        public AuthController(ILoginService loginService,
                              IMapper mapper)
        {
            _loginService = loginService;
            _mapper = mapper;
            
        }


        [HttpPost]
        [Route("login")]
        public ActionResult<LoginResponseDto> Login(Credentials credentials)
        {
            var domainResponse = _loginService.Login(credentials.Username,credentials.Password);

            var ret = _mapper.Map<LoginResponseDto>(domainResponse);
            
            return ret;
        }

        [HttpPost]
        [Route("verify")]
        public ActionResult<IEnumerable<string>> Verify()
        {
            return new string[] { "value1", "value2" };
        }



    }

}