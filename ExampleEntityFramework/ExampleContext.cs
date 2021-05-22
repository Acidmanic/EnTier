using System.Data.Common;
using Microsoft.EntityFrameworkCore;

namespace ExampleEntityFramework
{
    public class ExampleContext:DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=Example.db");
        }
    }
}