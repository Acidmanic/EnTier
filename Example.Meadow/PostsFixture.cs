using Acidmanic.Utilities.DataTypes;
using EnTier.Repositories;
using Example.Meadow.Models;
using Microsoft.Extensions.Hosting.Internal;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.LightWeight;

namespace Example.Meadow;

public class PostsFixture
{
    public void Setup(ICrudRepository<Post, long> repository)
    {
        repository.Add(new Post
        {
            Title = "First",
            Content = "Lorem ipsum dolor sit amet",
            Date = TimeStamp.Now
        });
        repository.Add(new Post
        {
            Title = "Second",
            Content = "consectetur adipiscing elit",
            Date = TimeStamp.Now + 1000
        });
        var middleGuy = repository.Add(new Post
        {
            Title = "Third",
            Content = "sed do eiusmod tempor incididunt",
            Date = TimeStamp.Now + 2000
        });
        
        
        new ConsoleLogger().LogInformation("Middle value for Date is: {MiddleDate}",middleGuy.Date.TotalMilliSeconds);
        
        repository.Add(new Post
        {
            Title = "Fourth",
            Content = "ut labore et dolore magna aliqua",
            Date = TimeStamp.Now + 3000
        });
        repository.Add(new Post
        {
            Title = "Fifth",
            Content = "Ut enim ad minim veniam",
            Date = TimeStamp.Now + 4000
        });
    }
}