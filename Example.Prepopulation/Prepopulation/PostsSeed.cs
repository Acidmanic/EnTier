using Acidmanic.Utilities.Results;
using EnTier.Prepopulation;
using EnTier.Prepopulation.Attributes;
using EnTier.UnitOfWork;
using Example.Prepopulation.Models;

namespace Example.Prepopulation.Prepopulation
{
    [DependsOnSeed(typeof(UsersSeed))]
    public class PostsSeed:PrepopulationSeedBase<Post,long>
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
        
        
        
        public PostsSeed(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public override Result Seed()
        {
            return SeedAll(new[] { FirstPost, SecondPost });
        }

        public override void Clear()
        {
            
        }
    }
}