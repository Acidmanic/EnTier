using EnTier;
using EnTier.Controllers;
using Example.EventSourcing.EntityFramework.DomainModels;
using Example.EventSourcing.EntityFramework.EventSourcing;
using Microsoft.AspNetCore.Mvc;

namespace Example.EventSourcing.EntityFramework.Controllers
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