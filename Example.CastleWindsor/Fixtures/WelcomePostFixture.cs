using EnTier.Repositories;
using Example.CastleWindsor.Contracts;
using Example.CastleWindsor.Models;

namespace Example.CastleWindsor.Fixtures
{
    public class WelcomePostFixture
    {

        private ITitleSuggestionService _titleSuggestionService;

        public WelcomePostFixture(ITitleSuggestionService titleSuggestionService)
        {
            _titleSuggestionService = titleSuggestionService;
        }

        public void Setup(ICrudRepository<Post, long> postsRepository)
        {
            postsRepository.Add(new Post
            {
                Id = 0,
                Content = "Welcome to posts blog! This is full of posts! Posts everywhere!",
                Title = "Welcome"
            });
            var randomContent = "Warning: The Samsung Galaxy S III (International) is no longer maintained. A build guide is available for developers that would like to make private builds, or even restart official support.";
            
            postsRepository.Add(new Post
            {
                Id=1,
                Content = randomContent,
                Title = _titleSuggestionService.SuggestTitleFor(randomContent)
            });
        }
    }
}