using EnTier.DataAccess.EntityFramework.Models;
using ExampleEntityFramework.StoragesModels;
using Microsoft.EntityFrameworkCore;

namespace ExampleEntityFramework
{
    public class ExampleContext:DbContext
    {
        public DbSet<PostStg> Posts { get; set; }
        
        public DbSet<MarkedFilterResult<PostStg,long>> PostsFilterResults { get; set; }
        
        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=Example.db");
        }
    }
}