﻿using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace PollyPocAPIApplication.Controllers.PollyBasicCircuitBreakerExample
{
    [Produces("application/json")]
    [Route("api/circuitbreaker/Inventory")]
    public class InventoryController : Controller
    {
        static int _requestCount = 0;

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            await Task.Delay(100); // simulate some data processing by delaying for 100 milliseconds 
            _requestCount++;

            if (_requestCount % 4 == 0) // only one of out four requests will succeed
            {
                return Ok(15);
            }

            return StatusCode((int) HttpStatusCode.InternalServerError, "Something went wrong");
        }
    }
}
