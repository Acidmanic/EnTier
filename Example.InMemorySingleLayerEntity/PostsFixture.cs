using EnTier.Extensions;
using EnTier.Repositories;
using EnTier.UnitOfWork;
using ExampleInMemorySingleLayerEntity.Models;

namespace ExampleInMemorySingleLayerEntity
{
    public class PostsFixture
    {
        public void Setup(ICrudRepository<Post, string> repository,IUnitOfWork unitOfWork)
        {
            repository.Add(new Post
            {
                Content = "First Content",
                Title = "First title"
            });
            repository.Add(new Post
            {
                Content = "Second Content",
                Title = "Second title"
            });
            repository.Add(new Post
            {
                Content = "Third Content",
                Title = "Third title"
            });
            repository.Add(new Post
            {
                Content = "Fourth Content",
                Title = "Fourth title"
            });
            repository.Add(new Post
            {
                Content = "Fifth Content",
                Title = "Fifth title"
            });
            
            unitOfWork.UpdateIndexes<Post,string>(false);

        }
    }
}