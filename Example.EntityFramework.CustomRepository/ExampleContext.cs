using System.Data.Common;
using EnTier.Filtering.Models;
using EnTier.Repositories.Models;
using ExampleEntityFramework.StoragesModels;
using Microsoft.EntityFrameworkCore;

namespace ExampleEntityFramework
{
    public class ExampleContext:DbContext
    {
        public DbSet<PostStg> Posts { get; set; }
        public DbSet<FilterResult> FilterResults { get; set; }
        

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=Example.db");
        }
    }
}