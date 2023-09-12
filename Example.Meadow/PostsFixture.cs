using Acidmanic.Utilities.DataTypes;
using EnTier.Extensions;
using EnTier.Repositories;
using EnTier.UnitOfWork;
using Example.Meadow.Models;
using Microsoft.Extensions.Hosting.Internal;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.LightWeight;

namespace Example.Meadow;

public class PostsFixture
{
    public void Setup(
        ICrudRepository<Post, long> repository,
        ICrudRepository<Media, long> mediaRepository,
        ILogger logger,
        IUnitOfWork unitOfWork)
    {


        mediaRepository.Add(new Media
        {
            Name = "Fascinating",
            Url = "http://acidmanic.com/live"
        });
        mediaRepository.Add(new Media
        {
            Name = "Beautiful",
            Url = "http://acidmanic.com/live"
        });
        mediaRepository.Add(new Media
        {
            Name = "Great",
            Url = "http://acidmanic.com/live"
        });
        
        repository.Add(new Post
        {
            Title = "First",
            Content = "Lorem ipsum dolor sit amet",
            Date = TimeStamp.Now,
            MediaId = 1
        });
        repository.Add(new Post
        {
            Title = "Second",
            Content = "consectetur adipiscing elit",
            Date = TimeStamp.Now + 1000,
            MediaId = 1
        });
        var middleGuy = repository.Add(new Post
        {
            Title = "Third",
            Content = "sed do eiusmod tempor incididunt",
            Date = TimeStamp.Now + 2000,
            MediaId = 2
        });
        
        
        logger.LogInformation("Middle value for Date is: {MiddleDate}",middleGuy.Date.TotalMilliSeconds);
        
        repository.Add(new Post
        {
            Title = "Fourth",
            Content = "ut labore et dolore magna aliqua",
            Date = TimeStamp.Now + 3000,
            MediaId = 2
        });
        repository.Add(new Post
        {
            Title = "Fifth",
            Content = "Ut enim ad minim veniam",
            Date = TimeStamp.Now + 4000,
            MediaId = 3
        });
        
        unitOfWork.UpdateIndexes<Post,long>(true);
    }
}