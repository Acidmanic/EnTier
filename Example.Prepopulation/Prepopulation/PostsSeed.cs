using System;
using System.Collections.Generic;
using Acidmanic.Utilities.Results;
using EnTier.Prepopulation.Attributes;
using EnTier.Prepopulation.Contracts;
using Example.Prepopulation.Models;

namespace Example.Prepopulation.Prepopulation
{
    [SeedIndex]
    [DependsOnSeed(typeof(UsersSeed))]
    public class PostsSeed : ISeed<Post>
    {
        public static Post FirstPost = new Post
        {
            Content = "This is first post",
            Title = "First",
            UserId = UsersSeed.Administrator.Id
        };

        public static Post SecondPost = new Post
        {
            Content = "This is second post",
            Title = "Second",
            UserId = UsersSeed.Administrator.Id
        };


        public string SeedName => "Posts";

        public IEnumerable<Post> SeedingObjects => new[] { FirstPost, SecondPost };

        public Result<ISeedingHook<Post>> HooksIntoSeedingBehavior =>
            new Result<ISeedingHook<Post>>().FailAndDefaultValue();

        public void Initialize()
        {
            Console.WriteLine("Initialized posts seeding");
        }
    }
}