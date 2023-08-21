using System.Collections.Generic;
using EnTier.DataAccess.EntityFramework;
using EnTier.Filtering.Models;
using EnTier.Repositories.Models;
using ExampleEntityFramework.StoragesModels;
using Microsoft.EntityFrameworkCore;

namespace ExampleEntityFramework
{
    public class DummyRepository:EntityFrameWorkCrudRepository<PostStg,long>
    {
        public DummyRepository(DbSet<PostStg> dbSet,DbSet<FilterResult> filterResults) : base(dbSet,filterResults)
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

        public override IEnumerable<PostStg> All()
        {
            return new List<PostStg>()
            {
                new PostStg
                {
                    Content = "This Content are coming from override method in custom repository",
                    Title = "CustomRepositoryUsed",
                    PostStgId = 1
                }
            };
        }
    }
}