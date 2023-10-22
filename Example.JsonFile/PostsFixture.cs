using EnTier.Extensions;
using EnTier.Repositories;
using EnTier.UnitOfWork;
using ExampleJsonFile.Storage;

namespace ExampleJsonFile
{
    public class PostsFixture
    {
        public void Setup(ICrudRepository<PostStg, long> repository,IUnitOfWork unitOfWork)
        {
            repository.Add(new PostStg
            {
                Content = "First Content",
                Title = "First title"
            });
            repository.Add(new PostStg
            {
                Content = "Second Content",
                Title = "Second title"
            });
            repository.Add(new PostStg
            {
                Content = "Third Content",
                Title = "Third title"
            });
            repository.Add(new PostStg
            {
                Content = "Fourth Content",
                Title = "Fourth title"
            });
            repository.Add(new PostStg
            {
                Content = "Fifth Content",
                Title = "Fifth title"
            });
            
            unitOfWork.UpdateIndexes<PostStg,long>(false);

        }
    }
}