using System.Collections.Generic;
using EnTier.DataAccess.EntityFramework;
using ExampleEntityFramework.StoragesModels;
using Microsoft.EntityFrameworkCore;

namespace ExampleEntityFramework
{
    public class DummyRepository:EntityFrameWorkCrudRepository<PostStg,long>
    {
        public DummyRepository(DbSet<PostStg> dbSet) : base(dbSet)
        {
        }

        public List<PostStg> GetNoneExistingPosts()
        {
            return new List<PostStg>()
            {
                new PostStg
                {
                    Content = "This does not exists in database",
                    Title = "NoneExisting",
                    PostStgId = 0
                }
            };
        }
        
        
        
    }
}