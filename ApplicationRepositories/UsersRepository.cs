



using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Repository;
using StorageModels;

namespace ApplicationRepositories
{
    public class UsersRepository : RepositoryBase<User>, IUsersRepository
    {
        public UsersRepository(DbSet<User> dbset) : base(dbset)
        {
        }


    }
}