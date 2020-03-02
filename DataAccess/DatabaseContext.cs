
using Microsoft.EntityFrameworkCore;
using StorageModels;

namespace DataAccess
{
    public class DataBaseContext:DbContext{


        public DataBaseContext()
        {
            
        }

        protected override void OnConfiguring (DbContextOptionsBuilder optionsBuilder){

            optionsBuilder.UseSqlServer("Data Source=.\\SQLSERVER; Initial Catalog=ArtworkDb;User Id=sa;Password=never54aga!n");
        }

        public DbSet<User> Users {get;set;}

    }
}

