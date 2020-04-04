
using System.Collections.Generic;
using DataAccess;
using DomainModels;
using Repository;
using EnTier.Utility;
using EnTier.Binding;
using EnTier.Service;
using EnTier.Binding.Abstractions;
using EnTier.DataAccess;
using Microsoft.EntityFrameworkCore;
using EnTier.Annotations;

namespace  Service
{
    public class UsersService : ServiceBase<StorageModels.User,User>, 
        IUsersService
    {
        public UsersService(IObjectMapper mapper) : base(mapper)
        {
        }


        public User AddUser(User user)
        {
            var storage = Mapper.Map<StorageModels.User>(user);

            using(var db = DbProvider.Create()){

                storage = db.GetRepository<StorageModels.User,long>().Add(storage);
                
                db.Compelete();
            }

            return Mapper.Map<User>(storage);
        }

        List<User> IUsersService.GetAll()
        {

            return new List<User>();
            // return base.GetAll();
        }
    }
}