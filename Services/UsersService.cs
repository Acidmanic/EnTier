



using System.Collections.Generic;
using AutoMapper;
using DataAccess;
using DomainModels;

namespace  Services
{
    public class UsersService : ServiceBase, IUsersService
    {
        public UsersService(IMapper mapper, IProvider<DatabaseUnit> dbProvider) : base(mapper, dbProvider)
        {
        }

        public User AddUser(User user)
        {

            var storage = Mapper.Map<StorageModels.User>(user);

            using(var db = DbProvider.Create()){

                storage = db.Users.Add(storage);
                
                db.Compelete();
            }

            return Mapper.Map<User>(storage);
        }

        public List<User> GetAll()
        {
            List<StorageModels.User> res = null;

            using (var db = DbProvider.Create()){

                res = db.Users.GetAll();
            }

            var ret = Mapper.Map<List<User>>(res);

            return ret;
        }
    }
}