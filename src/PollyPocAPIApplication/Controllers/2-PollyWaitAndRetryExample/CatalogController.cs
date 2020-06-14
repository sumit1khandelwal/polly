using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Polly;
using Polly.Retry;

namespace PollyPocAPIApplication.Controllers.PollyWaitAndRetryExample
{
    [Produces("application/json")]
    [Route("api/waitnretry/[controller]")]
    public class CatalogController : Controller
    {
        readonly RetryPolicy<HttpResponseMessage> _httpWaitNRetryPolicy;
        private readonly IHttpClientFactory _httpClientFactory;
        public CatalogController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;

            _httpWaitNRetryPolicy =
                Policy.HandleResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
                    .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt) / 2), (ex, span, retryCount, context)=>
                    {
                        Console.WriteLine($"Retrying..{retryCount} at {span}");
                    });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var httpClient = _httpClientFactory.CreateClient("InventoryClient");
            string requestEndpoint = $"waitnretry/inventory/{id}";

            HttpResponseMessage response = await _httpWaitNRetryPolicy.ExecuteAsync(() => httpClient.GetAsync(requestEndpoint));

            if (response.IsSuccessStatusCode)
            {
                int itemsInStock = JsonConvert.DeserializeObject<int>(await response.Content.ReadAsStringAsync());
                return Ok(itemsInStock);
            }

            return StatusCode((int) response.StatusCode, response.Content.ReadAsStringAsync());
        }
    }
}
