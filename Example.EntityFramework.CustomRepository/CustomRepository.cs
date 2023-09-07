using System.Collections.Generic;
using EnTier.DataAccess.EntityFramework;
using EnTier.DataAccess.EntityFramework.FullTreeHandling;
using EnTier.DataAccess.JsonFile;
using ExampleEntityFramework.StoragesModels;
using Microsoft.EntityFrameworkCore;

namespace ExampleEntityFramework
{
    public class DummyRepository : EntityFrameWorkCrudRepository<PostStg, long>
    {
        
        public DummyRepository(DbSet<PostStg> dbSet, DbSet<MarkedFilterResult<PostStg, long>> filterResults, DbSet<MarkedSearchIndex<PostStg, long>> searchIndex, IFullTreeMarker<PostStg> fullTreeMarker) : base(dbSet, filterResults, searchIndex, fullTreeMarker)
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