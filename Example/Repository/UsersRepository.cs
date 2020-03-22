



using System.Collections.Generic;
using Context;
using Microsoft.EntityFrameworkCore;
using Repository;
using StorageModels;

namespace ApplicationRepositories
{
    public class UsersRepository : RepositoryBase<User>, IUsersRepository
    {
        public UsersRepository(IDataset<User> dataset) : base(dataset)
        {
        }


    }
}