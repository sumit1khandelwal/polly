using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Polly;
using Polly.Caching;
using Polly.Registry;
// ReSharper disable All

namespace PollyCachingExample.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class CatalogController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly CachePolicy<HttpResponseMessage> _cachePolicy;

        public CatalogController(IPolicyRegistry<string> myRegistry, HttpClient httpClient)
        {
            _httpClient = httpClient;
            _cachePolicy = myRegistry.Get<CachePolicy<HttpResponseMessage>>("myLocalCachePolicy");
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            string requestEndpoint = $"inventory/{id}";

            Context policyExecutionContext = new Context($"GetInventoryById-{id}");

            HttpResponseMessage response = await _cachePolicy.ExecuteAsync(
                () => _httpClient.GetAsync(requestEndpoint), policyExecutionContext);

            if (response.IsSuccessStatusCode)
            {
                int itemsInStock = JsonConvert.DeserializeObject<int>(await response.Content.ReadAsStringAsync());
                return Ok(itemsInStock);
            }

            return StatusCode((int)response.StatusCode, response.Content.ReadAsStringAsync());
        }

    }
}
