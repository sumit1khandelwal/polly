using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Polly;
using Polly.Fallback;
using Polly.Retry;
using Polly.Timeout;
using Polly.Wrap;

namespace PollyPocAPIApplication.Controllers.PollyWrappingGenericsExample
{
    [Produces("application/json")]
    [Route("api/policywrap/[controller]")]
    public class CatalogController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly PolicyWrap<HttpResponseMessage> _policyWrap;

        public CatalogController(IHttpClientFactory httpClientFactory,PolicyWrap<HttpResponseMessage> policyWrap)
        {
            _httpClientFactory = httpClientFactory;
            _policyWrap = policyWrap;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var httpClient = _httpClientFactory.CreateClient("InventoryClient");
            string requestEndpoint = $"policywrap/inventory/{id}";

            HttpResponseMessage response = await _policyWrap.ExecuteAsync(
                async token => await httpClient.GetAsync(requestEndpoint, token), CancellationToken.None);

            if (response.IsSuccessStatusCode)
            {
                int itemsInStock = JsonConvert.DeserializeObject<int>(await response.Content.ReadAsStringAsync());
                return Ok(itemsInStock);
            }

            if (response.Content != null)
            {
                return StatusCode((int)response.StatusCode, response.Content.ReadAsStringAsync());
            }
            return StatusCode((int)response.StatusCode);
        }

      
        
    }
}
