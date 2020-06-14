using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace PollyPocAPIApplication.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class ExternalApiController : Controller
    {
        #region Comments
        //NETSTAT.EXE | findstr waws-prod-bay-017:https
        //NETSTAT.EXE | findstr waws-prod-bay-017:http
        #endregion

        private readonly IHttpClientFactory _httpClientFactory;
        public ExternalApiController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        #region Option 1
        //http://localhost:57697/api/externalapi/option1
        [HttpGet("option1")]
        public async Task CallExternalApi()
        {
            Console.WriteLine("Starting connections");
            for (int i = 0 ; i < 10 ; i++)
            {
                using (var client = new HttpClient())
                {
                    var result = await client.GetAsync("http://aspnetmonsters.com/");
                    Console.WriteLine(result.StatusCode);
                }
            }
            Console.WriteLine("Connections done");
        }
        #endregion

        #region Option 2
        //http://localhost:57697/api/externalapi/option2
        static HttpClient Client = new HttpClient();
        [HttpGet("option2")]
        public async Task CallExternalApiWithStaticClient()
        {
            Console.WriteLine("Starting connections");
            for (int i = 0 ; i < 10 ; i++)
            {
                var result = await Client.GetAsync("http://aspnetmonsters.com/");
                Console.WriteLine(result.StatusCode);
            }
            Console.WriteLine("Connections done");
            Console.ReadLine();
        }
        #endregion

        #region Option 3
        ////http://localhost:57697/api/externalapi/option3
        [HttpGet("option3")]
        public async Task CallExternalApiWithHttpClientFactoryClient()
        {
            var httpClient = _httpClientFactory.CreateClient("RemoteServer");
            Console.WriteLine("Starting connections");
            for (int i = 0 ; i < 10 ; i++)
            {
                var result = await httpClient.GetAsync("/");
                Console.WriteLine(result.StatusCode);
            }
            Console.WriteLine("Connections done");
            Console.ReadLine();
        }
        #endregion

    }
}
