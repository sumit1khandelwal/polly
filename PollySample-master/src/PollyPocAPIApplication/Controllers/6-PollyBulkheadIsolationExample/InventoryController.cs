using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace PollyPocAPIApplication.Controllers.PollyBulkheadIsolationExample
{
    [Produces("application/json")]
    [Route("api/bulkhead/Inventory")]
    public class InventoryController : Controller
    {
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            await Task.Delay(10000); // simulate some data processing by delaying for 10 seconds 

            return Ok(15);
        }
    }
}
