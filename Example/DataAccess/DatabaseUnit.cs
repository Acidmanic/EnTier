





using ApplicationRepositories;
using Microsoft.EntityFrameworkCore;
using Repository;

namespace DataAccess
{
    public class DatabaseUnit : DatabaseContextUnitOfWorkBase
    {


        public IUsersRepository Users {get;private set;}

        public DatabaseUnit(DataBaseContext context) : base(context)
        {

            Users = new UsersRepository(context.Users);
        }
    }
}
