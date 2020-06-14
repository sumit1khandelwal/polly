using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Polly;
using Polly.Registry;

namespace PollyPocAPIApplication.Controllers.PollyRegistryExample
{
    [Produces("application/json")]
    [Route("api/registry/[controller]")]
    public class CatalogController : Controller
    {
        readonly IPolicyRegistry<string> _policyRegistry;
        private readonly IHttpClientFactory _httpClientFactory;

        public CatalogController(IPolicyRegistry<string> policyRegistry, IHttpClientFactory httpClientFactory)
        {
            _policyRegistry = policyRegistry;
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var httpClient = _httpClientFactory.CreateClient("InventoryClient");
            string requestEndpoint = $"registry/inventory/{id}";

            IAsyncPolicy<HttpResponseMessage> httpRetryPolicy = 
                _policyRegistry.Get<IAsyncPolicy<HttpResponseMessage>>("SimpleHttpRetryPolicy");

            IAsyncPolicy httpClientTimeoutExceptionPolicy = 
                _policyRegistry.Get<IAsyncPolicy>("SimpleHttpTimeoutPolicy");

            HttpResponseMessage response =
                await httpRetryPolicy.ExecuteAsync(
                    () => httpClientTimeoutExceptionPolicy.ExecuteAsync(
                        async token => await httpClient.GetAsync(requestEndpoint, token), CancellationToken.None));

            if (response.IsSuccessStatusCode)
            {
                int itemsInStock = JsonConvert.DeserializeObject<int>(await response.Content.ReadAsStringAsync());
                return Ok(itemsInStock);
            }

            return StatusCode((int)response.StatusCode, response.Content.ReadAsStringAsync());
        }
    }
}
