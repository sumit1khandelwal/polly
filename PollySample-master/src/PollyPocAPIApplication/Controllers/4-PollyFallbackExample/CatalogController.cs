using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Polly;
using Polly.Fallback;
using Polly.Retry;

namespace PollyPocAPIApplication.Controllers.PollyFallbackExample
{
    [Produces("application/json")]
    [Route("api/fallback/[controller]")]
    public class CatalogController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        readonly RetryPolicy<HttpResponseMessage> _httpRetryPolicy;
        readonly FallbackPolicy<HttpResponseMessage> _httpRequestFallbackPolicy;

        private int _cachedNumber = 0;

        public CatalogController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
            _httpRetryPolicy = Policy.HandleResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode).RetryAsync(3);

            _httpRequestFallbackPolicy =
                Policy.HandleResult<HttpResponseMessage>(r => r.StatusCode == HttpStatusCode.InternalServerError)
                    .FallbackAsync(
                        new HttpResponseMessage(HttpStatusCode.OK)
                        {
                            Content = new ObjectContent(_cachedNumber.GetType(),
                            _cachedNumber, new JsonMediaTypeFormatter())
                        });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var httpClient = _httpClientFactory.CreateClient("InventoryClient");
            string requestEndpoint = $"fallback/inventory/{id}";

            HttpResponseMessage response = await _httpRequestFallbackPolicy.ExecuteAsync(
                 () => _httpRetryPolicy.ExecuteAsync(
                     () => httpClient.GetAsync(requestEndpoint)));

            if (response.IsSuccessStatusCode)
            {
                int itemsInStock = JsonConvert.DeserializeObject<int>(await response.Content.ReadAsStringAsync());
                return Ok(itemsInStock);
            }

            return StatusCode((int)response.StatusCode, response.Content.ReadAsStringAsync());
        }
    }
}
