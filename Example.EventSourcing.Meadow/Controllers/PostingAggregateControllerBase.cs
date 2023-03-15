using EnTier;
using EnTier.Controllers;
using Example.EventSourcing.Meadow.DomainModels;
using Example.EventSourcing.Meadow.EventSourcing;
using Microsoft.AspNetCore.Mvc;

namespace Example.EventSourcing.Meadow.Controllers
{
    
    [ApiController]
    [Route("posting")]
    public class PostingAggregateControllerBase : AggregateControllerBase<Post,IPostEvent,long,long>
    {
        public PostingAggregateControllerBase(EnTierEssence essence) : base(essence)
        {
        }

        protected override bool ReflectExceptions => true;
    }
}