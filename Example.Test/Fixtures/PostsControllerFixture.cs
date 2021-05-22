using System;
using EnTier.Repositories;
using ExampleJsonFile.Storage;

namespace Example.UnitTest.Fixtures
{
    public class PostsControllerFixture
    {
        public void Setup(ICrudRepository<PostStg, string> repo)
        {
            repo.Add(new PostStg()
            {
                Content = "This value is seeded Into Database.",
                Title = "DesiredTitle",
                Id = "A"
            });
            repo.Add(new PostStg()
            {
                Content = "This value is seeded Into Database.",
                Title = "NormalTitle",
                Id = "B"
            });
        }
    }
}