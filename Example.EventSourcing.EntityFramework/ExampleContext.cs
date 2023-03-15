using EnTier.DataAccess.EntityFramework;
using Example.EventSourcing.EntityFramework.StoragesModels;
using Microsoft.EntityFrameworkCore;

namespace Example.EventSourcing.EntityFramework
{
    public class ExampleContext:DbContext
    {
        public DbSet<PostStg> Posts { get; set; }
        
        public DbSet<EfObjectEntry<long,long>> PostsEvents { get; set; }
        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=Example.db");
        }
    }
}