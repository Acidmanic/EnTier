using EnTier;
using EnTier.Controllers;
using ExampleInMemorySingleLayerEntity.Models;
using Microsoft.AspNetCore.Mvc;

namespace ExampleInMemorySingleLayerEntity.Controllers
{
    [ApiController]
    [Route("products")]
    public class ProductsController:CrudControllerBase<Product,long>
    {
        public ProductsController(EnTierEssence essence) : base(essence)
        {
        }
    }
}