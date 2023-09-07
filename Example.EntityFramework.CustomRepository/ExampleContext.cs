using EnTier.DataAccess.JsonFile;
using ExampleEntityFramework.StoragesModels;
using Microsoft.EntityFrameworkCore;

namespace ExampleEntityFramework
{
    public class ExampleContext:DbContext
    {
        public DbSet<PostStg> Posts { get; set; }
        
        public DbSet<MarkedFilterResult<PostStg,long>> PostsFilterResults { get; set; }
        public DbSet<MarkedSearchIndex<PostStg,long>> PostsSearchIndex { get; set; }
        

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=Example.db");
        }
    }
}