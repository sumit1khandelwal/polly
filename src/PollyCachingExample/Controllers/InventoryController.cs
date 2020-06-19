using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace PollyCachingExample.Controllers
{
    [Produces("application/json")]
    [Route("api/Inventory")]
    public class InventoryController : Controller
    {
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            await Task.Delay(100);// simulate some data processing by delaying for 100 milliseconds 
            return Ok(id + 100);
        }
    }
}
