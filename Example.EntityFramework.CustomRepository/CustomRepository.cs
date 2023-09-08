using System.Collections.Generic;
using EnTier.DataAccess.EntityFramework;
using ExampleEntityFramework.StoragesModels;

namespace ExampleEntityFramework
{
    public class DummyRepository : EntityFrameWorkCrudRepository<PostStg, long>
    {
        
        public DummyRepository(ExampleContext dbContext) : base(dbContext.Posts,
            dbContext.PostsFilterResults, dbContext.PostsSearchIndex, null)
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