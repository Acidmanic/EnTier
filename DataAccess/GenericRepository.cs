



using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Repository;

namespace DataAccess
{

    public partial class GenericDatabaseUnit
    {
        private class GenericRepository<T> : RepositoryBase<T> where T : class
        {
            public GenericRepository(DbSet<T> dbset) : base(dbset)
            {
            }
        }

            
    }


    
}