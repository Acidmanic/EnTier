using EnTier;
using EnTier.Controllers;
using ExampleEntityFramework.DomainModels;
using ExampleEntityFramework.EventSourcing;
using Microsoft.AspNetCore.Mvc;

namespace ExampleEntityFramework.Controllers
{
    
    [ApiController]
    [Route("posting")]
    public class PostingAggregateController : AggregateController<Post,IPostEvent,long,long>
    {
        public PostingAggregateController(EnTierEssence essence) : base(essence)
        {
        }
    }
}