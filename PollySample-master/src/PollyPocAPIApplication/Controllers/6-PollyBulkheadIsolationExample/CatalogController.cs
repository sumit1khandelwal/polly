using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Polly.Bulkhead;

namespace PollyPocAPIApplication.Controllers.PollyBulkheadIsolationExample
{
    [Produces("application/json")]
    [Route("api/bulkhead/[controller]")]
    public class CatalogController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private static int _requestCount = 0;
        private readonly BulkheadPolicy<HttpResponseMessage> _bulkheadIsolationPolicy;

        public CatalogController(IHttpClientFactory httpClientFactory, BulkheadPolicy<HttpResponseMessage> bulkheadIsolationPolicy)
        {
            _httpClientFactory = httpClientFactory;
            _bulkheadIsolationPolicy = bulkheadIsolationPolicy;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var httpClient = _httpClientFactory.CreateClient("InventoryClient");
            _requestCount++;
            LogBulkheadInfo();
            string requestEndpoint = $"bulkhead/inventory/{id}";

            HttpResponseMessage response = await _bulkheadIsolationPolicy.ExecuteAsync(
                     () => httpClient.GetAsync(requestEndpoint));

            if (response.IsSuccessStatusCode)
            {
                int itemsInStock = JsonConvert.DeserializeObject<int>(await response.Content.ReadAsStringAsync());
                return Ok(itemsInStock);
            }

            return StatusCode((int)response.StatusCode, response.Content.ReadAsStringAsync());
        }

        private void LogBulkheadInfo()
        {
            Console.WriteLine("======================================");
            Console.WriteLine($"PollyDemo RequestCount {_requestCount}");
            Console.WriteLine($"PollyDemo BulkheadAvailableCount " +
                                               $"{_bulkheadIsolationPolicy.BulkheadAvailableCount}");
            Console.WriteLine($"PollyDemo QueueAvailableCount " +
                                               $"{_bulkheadIsolationPolicy.QueueAvailableCount}");
        }
    }
}
