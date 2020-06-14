using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace PollyPocAPIApplication.Controllers.PollySharingPoliciesByDI
{
    [Produces("application/json")]
    [Route("api/policies/[controller]")]
    public class CatalogController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly PolicyHolder _policyHolder;

        public CatalogController(PolicyHolder policyHolder, IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
            _policyHolder = policyHolder;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var httpClient = _httpClientFactory.CreateClient("InventoryClient");
            string requestEndpoint = $"policies/inventory/{id}";

            HttpResponseMessage response = await _policyHolder.HttpRetryPolicy
                                                    .ExecuteAsync(() => httpClient.GetAsync(requestEndpoint));

            if (response.IsSuccessStatusCode)
            {
                int itemsInStock = JsonConvert.DeserializeObject<int>(await response.Content.ReadAsStringAsync());
                return Ok(itemsInStock);
            }

            return StatusCode((int) response.StatusCode, response.Content.ReadAsStringAsync());
        }
    }
}
