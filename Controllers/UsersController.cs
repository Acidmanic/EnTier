





using DataAccess;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Repository;
using StorageModels;

namespace Controllers
{



    [Route("api/v1/{Controller}")]
    [ApiController]
    public class UsersController:ControllerBase
    {


        private IProvider<DatabaseUnit> _dbProvider;


        public UsersController(IProvider<DatabaseUnit> dbProvider)
        {
            _dbProvider = dbProvider;
        }

        [HttpPost]
        [Route("")]
        public User PostUser(User user){
            

            User ret = null;

            using(var db = _dbProvider.Create()){

                ret = db.Users.Add(user);

                db.Compelete();
            }

            return ret;
        }
    }
    
}