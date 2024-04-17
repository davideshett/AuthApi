using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace api.Controllers
{
    [Authorize]
    [ApiController]
    //[ApiVersion("1.0")]
    [Route("api/[controller]")]
    public class BaseController : ControllerBase
    {
        
    }
}