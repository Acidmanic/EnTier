



using System.Collections.Generic;
using EnTier.Context;
using EnTier.Repository;
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