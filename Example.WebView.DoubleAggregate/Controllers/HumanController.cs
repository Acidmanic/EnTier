using System;
using EnTier;
using EnTier.Controllers;
using Example.WebView.DoubleAggregate.Human;
using Microsoft.AspNetCore.Mvc;

namespace Example.WebView.DoubleAggregate.Controllers
{
    
    [ApiController]
    [Route("Humans")]
    public class HumanController:AggregateControllerBase<Human.Human,IHumanEvent,Guid,long>
    {
        public HumanController(EnTierEssence essence) : base(essence)
        {
        }
    }
}