using EnTier.Controllers;
using Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Example2.Controllers
{
    [Route("api/v1/Songs")]
    [ApiController]
    public class SongsController: EnTierControllerBase<Song, Song, Song, long>
    {


    }
}
