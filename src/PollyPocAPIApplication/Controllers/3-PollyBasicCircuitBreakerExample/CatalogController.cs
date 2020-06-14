using System;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Polly;
using Polly.CircuitBreaker;
using Polly.Retry;

namespace PollyPocAPIApplication.Controllers.PollyBasicCircuitBreakerExample
{
    [Produces("application/json")]
    [Route("api/circuitbreaker/[controller]")]
    public class CatalogController : Controller
    {
        readonly RetryPolicy<HttpResponseMessage> _httpRetryPolicy;
        private readonly IHttpClientFactory _httpClientFactory;

        private readonly CircuitBreakerPolicy<HttpResponseMessage> _breakerPolicy;
        public CatalogController(IHttpClientFactory httpClientFactory, CircuitBreakerPolicy<HttpResponseMessage> breakerPolicy)
        {
            _httpClientFactory = httpClientFactory;
            _breakerPolicy = breakerPolicy;
            _httpRetryPolicy = Policy
                                .HandleResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode).RetryAsync(2);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var httpClient = _httpClientFactory.CreateClient("InventoryClient");
            string requestEndpoint = $"circuitbreaker/inventory/{id}";
            HttpResponseMessage response = null;
                 response = await _httpRetryPolicy.ExecuteAsync(
                         () => _breakerPolicy.ExecuteAsync(
                             () => httpClient.GetAsync(requestEndpoint)));
                if (response.IsSuccessStatusCode)
                {
                    int itemsInStock = JsonConvert.DeserializeObject<int>(await response.Content.ReadAsStringAsync());
                    return Ok(itemsInStock);
                }
            return StatusCode((int) response.StatusCode, response.Content.ReadAsStringAsync());
        }
    }
}
