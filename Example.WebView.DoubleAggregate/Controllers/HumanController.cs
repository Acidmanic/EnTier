using System;
using EnTier;
using EnTier.Controllers;
using Example.WebView.DoubleAggregate.HumanArea;
using Microsoft.AspNetCore.Mvc;

namespace Example.WebView.DoubleAggregate.Controllers
{
    
    [ApiController]
    [Route("Humans")]
    public class HumanController:AggregateControllerBase<HumanArea.Human,IHumanEvent,Guid,long>
    {
        public HumanController(EnTierEssence essence) : base(essence)
        {
        }
    }
}