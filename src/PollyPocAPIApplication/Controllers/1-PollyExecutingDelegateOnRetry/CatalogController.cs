using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Polly;
using Polly.NoOp;
using Polly.Retry;

namespace PollyPocAPIApplication.Controllers.PollyExecutingDelegateOnRetry
{
    [Produces("application/json")]
    [Route("api/retry/[controller]")]
    public class CatalogController : Controller
    {
        readonly RetryPolicy<HttpResponseMessage> _httpRetryPolicy;
        private readonly IHttpClientFactory _httpClientFactory;
        public CatalogController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
            _httpRetryPolicy =
                Policy.
                HandleResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
                    .RetryAsync(3, onRetry: (httpResponseMessage, retryCount) =>
                    {
                        Console.WriteLine($"Retrying..{retryCount}");
                        if (httpResponseMessage.Result.StatusCode == HttpStatusCode.NotFound)
                        {
                            //log somewhere
                        }
                        else if (httpResponseMessage.Result.StatusCode == HttpStatusCode.Conflict)
                        {
                            //do something else
                        }
                    });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var httpClient = _httpClientFactory.CreateClient("InventoryClient");
            //string requestEndpoint = $"doesNotExist/";
            string requestEndpoint = $"retry/inventory/{id}";

            HttpResponseMessage response = await _httpRetryPolicy.ExecuteAsync(() => httpClient.GetAsync(requestEndpoint));

            if (response.IsSuccessStatusCode)
            {
                int itemsInStock = JsonConvert.DeserializeObject<int>(await response.Content.ReadAsStringAsync());
                return Ok(itemsInStock);
            }

            return StatusCode((int) response.StatusCode, response.Content.ReadAsStringAsync());
        }
    }
}
