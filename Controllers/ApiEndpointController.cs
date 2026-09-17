using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace UGB.Proyecto.Final.Controllers
{
    [ApiController]
    [EnableRateLimiting("ApiUsersPolicy")]
    public class ApiEndpointController : ControllerBase
    {
        
    }
}