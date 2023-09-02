using EnTier;
using EnTier.Controllers;
using Example.Meadow.Models;
using Microsoft.AspNetCore.Mvc;

namespace Example.Meadow.Controllers;

[ApiController]
[Route("medias")]
public class MediasController:CrudControllerBase<Media,long>
{
    public MediasController(EnTierEssence essence) : base(essence)
    {
    }
}