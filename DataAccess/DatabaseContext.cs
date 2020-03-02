


using Microsoft.EntityFrameworkCore;
using StorageModels;

namespace DataAccess
{
    public class DataBaseContext:DbContext{


        public DataBaseContext()
        {
            
        }

        protected override void OnConfiguring (DbContextOptionsBuilder optionsBuilder){
            
        }

        public DbSet<User> Users {get;set;}

    }
}

