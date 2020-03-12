



using System.Collections.Generic;
using DomainModels;

namespace Service
{
    

    public interface IUsersService{


        User AddUser(User user);

        List<User> GetAll();

    }
}