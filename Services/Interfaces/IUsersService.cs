



using System.Collections.Generic;
using DomainModels;

namespace Services
{
    

    public interface IUsersService{


        User AddUser(User user);

        List<User> GetAll();

    }
}