using EnTier;
using EnTier.Controllers;
using Example.Prepopulation.Models;
using Example.Prepopulation.Prepopulation;
using Microsoft.AspNetCore.Mvc;

namespace Example.Prepopulation.Controllers;



[ApiController]
[Route("chocolates")]
public class ChocolatesController:CrudControllerBase<Chocolate,long>
{
    public ChocolatesController(EnTierEssence essence) : base(essence)
    {
        
    }



    [HttpGet]
    [Route("known")]
    public IActionResult GetKnownChocolates()
    {
        return Ok(ChocolatesSeed.KnowsChocolates);
    }
}