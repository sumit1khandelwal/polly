using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace PollyPocAPIApplication.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        [HttpGet("")]
        // GET: Values
        public IActionResult Index()
        {
            return Ok(new { value = 1 });
        }
    }
}