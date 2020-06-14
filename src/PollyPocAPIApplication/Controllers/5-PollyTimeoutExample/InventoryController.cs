using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace PollyPocAPIApplication.Controllers.PollyTimeoutExample
{
    [Produces("application/json")]
    [Route("api/timeout/Inventory")]
    public class InventoryController : Controller
    {
        static int _requestCount = 0;

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            _requestCount++;

            if (_requestCount % 6 != 0)
            {
                await Task.Delay(10000); // simulate some data processing by delaying for 10 seconds
            }

            return Ok(15);
        }
    }
}
