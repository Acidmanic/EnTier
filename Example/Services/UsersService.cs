
using System.Collections.Generic;
using DataAccess;
using DomainModels;
using Repository;
using Utility;
using Plugging;


namespace  Service
{
    public class UsersService : ServiceBase<StorageModels.User,User>, 
        IUsersService
    {
        public UsersService(IObjectMapper mapper, IProvider<UnitOfDataAccessBase> dbProvider) : base(mapper, dbProvider)
        {
        }

        public UsersService(IObjectMapper mapper) : base(mapper, new DatabaseUnitProvider())
        {
        }


        public User AddUser(User user)
        {

            var storage = Mapper.Map<StorageModels.User>(user);

            using(var db = DbProvider.Create()){

                storage = db.GetRepository<StorageModels.User>().Add(storage);
                
                db.Compelete();
            }

            return Mapper.Map<User>(storage);
        }

        List<User> IUsersService.GetAll()
        {
            return base.GetAll();
        }
    }
}