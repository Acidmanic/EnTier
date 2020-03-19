
using Microsoft.EntityFrameworkCore;
using StorageModels;

namespace DataAccess
{
    public class DataBaseContext:DbContext{


        public DataBaseContext()
        {
            
        }

        protected override void OnConfiguring (DbContextOptionsBuilder optionsBuilder){

            optionsBuilder.UseSqlServer("Data Source=.; Initial Catalog=ArtworkDb;User Id=sa;Password=never54aga!n");
        }

        public DbSet<User> Users {get;set;}

        public DbSet<Post> Posts{get;set;}

        public DbSet<Project> Projects{get;set;}

    }
}
