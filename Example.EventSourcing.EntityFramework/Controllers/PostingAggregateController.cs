using EnTier;
using EnTier.Controllers;
using Example.EventSourcing.EntityFramework.DomainModels;
using Example.EventSourcing.EntityFramework.EventSourcing;
using Microsoft.AspNetCore.Mvc;

namespace Example.EventSourcing.EntityFramework.Controllers
{
    
    [ApiController]
    [Route("posting")]
    public class PostingAggregateController : AggregateController<Post,IPostEvent,long,long>
    {
        public PostingAggregateController(EnTierEssence essence) : base(essence)
        {
        }

        protected override bool ReflectExceptions => true;
    }
}