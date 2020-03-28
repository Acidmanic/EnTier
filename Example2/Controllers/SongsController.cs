using Controllers;
using Example2.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Example2.Controllers
{
    [Route("api/v1/[controller]")]
    public class SongsController: EnTierControllerBase<Song, Song, Song, long>
    {


    }
}
