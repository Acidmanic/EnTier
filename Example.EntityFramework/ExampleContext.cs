using System.Data.Common;
using EnTier.DataAccess.EntityFramework;
using ExampleEntityFramework.StoragesModels;
using Microsoft.EntityFrameworkCore;

namespace ExampleEntityFramework
{
    public class ExampleContext:DbContext
    {
        public DbSet<PostStg> Posts { get; set; }
        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=Example.db");
        }
    }
}