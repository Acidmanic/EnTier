





using System.Collections.Generic;
using DataAccess;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Repository;
using DataTransferModels;
using AutoMapper;
using Service;
using System;
using ApplicationRepositories;

namespace Controllers
{



    [Route("api/v1/{Controller}")]
    [ApiController]
    public class UsersController:ControllerBase
    {

        private readonly IUsersService _service;
        private readonly IMapper _mapper;


        public UsersController(
            IUsersService service,
            IMapper mapper)
        {
            _service = service;
            _mapper = mapper;
        }

        [HttpPost]
        [Route("")]
        public User PostUser(User user){
            
            var domain = _mapper.Map<DomainModels.User>(user);

            domain = _service.AddUser(domain);

            var ret = _mapper.Map<User>(domain);

            return ret;
        }

        [HttpGet]
        [Route("")]
        public List<User> GetAll(){

            var res = _service.GetAll();

            var ret = _mapper.Map<List<User>>(res);

            return ret;

        }


        
        [HttpGet]
        [Route("test2")]
        public object Test2(){

            return new {Name="Mani"};

        }
    }
    
}