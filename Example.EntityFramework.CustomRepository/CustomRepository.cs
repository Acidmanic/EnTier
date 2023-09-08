using System.Collections.Generic;
using EnTier.DataAccess.EntityFramework;
using EnTier.DataAccess.EntityFramework.FullTreeHandling;
using ExampleEntityFramework.StoragesModels;
using Microsoft.EntityFrameworkCore;

namespace ExampleEntityFramework
{
    public class DummyRepository : EntityFrameWorkCrudRepositoryBase<PostStg, long>
    {
        
        public DummyRepository(DbContext dbContext) : base(dbContext)
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

        public override IEnumerable<PostStg> All(bool readFullTree = false)
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