using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace PollyPocAPIApplication.Controllers.PollyFallbackExample
{
    [Produces("application/json")]
    [Route("api/fallback/Inventory")]
    public class InventoryController : Controller
    {
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            await Task.Delay(100);// simulate some data processing by delaying for 100 milliseconds 

            return StatusCode((int)HttpStatusCode.InternalServerError, "Something went wrong");
        }
    }
}
