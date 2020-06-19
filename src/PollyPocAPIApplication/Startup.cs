using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.CircuitBreaker;
using Polly.Registry;
using Polly.Bulkhead;
using Polly.Timeout;
using System.Net;
using System.Net.Http.Formatting;
using Polly.Wrap;
using Polly.Caching;
using Microsoft.Extensions.Caching.Memory;

namespace PollyPocAPIApplication
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            #region Other ways of injecting policies
            //IAsyncPolicy<HttpResponseMessage> httpRetryPolicy =
            //   Policy.HandleResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode).RetryAsync(3);
            //services.AddHttpClient("RemoteServer", client =>
            //{
            //    client.BaseAddress = new Uri("http://aspnetmonsters.com");
            //    //client.DefaultRequestHeaders.Add("Accept", "application/json");
            //}).AddPolicyHandler(httpRetryPolicy);
            #endregion

            #region Advanced Circuit Breaker
            //CircuitBreakerPolicy<HttpResponseMessage> breakerPolicy = Policy
            //  .HandleResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
            //  .AdvancedCircuitBreakerAsync(0.5, TimeSpan.FromSeconds(60), 7, TimeSpan.FromSeconds(15),
            //      OnBreak, OnReset, OnHalfOpen); 
            #endregion


            CircuitBreakerPolicy<HttpResponseMessage> breakerPolicy = Policy
                .HandleResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
                .CircuitBreakerAsync(2, TimeSpan.FromSeconds(10), OnBreak, OnReset, OnHalfOpen);


            BulkheadPolicy<HttpResponseMessage> bulkheadIsolationPolicy = Policy
               .BulkheadAsync<HttpResponseMessage>(2, 4, onBulkheadRejectedAsync: OnBulkheadRejectedAsync);


            services.AddHttpClient("RemoteServer", client =>
            {
                client.BaseAddress = new Uri("http://aspnetmonsters.com");
                //client.DefaultRequestHeaders.Add("Accept", "application/json");
            });
            services.AddHttpClient("InventoryClient", client =>
            {
                client.BaseAddress = new Uri("http://localhost:57697/api/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Add("Accept", "application/json");
            }).
            ConfigurePrimaryHttpMessageHandler(handler => new HttpClientHandler
            {
                AutomaticDecompression = DecompressionMethods.GZip
            });

            IAsyncPolicy<HttpResponseMessage> httpRetryPolicy =
                Policy.HandleResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
                    .RetryAsync(3);

            services.AddSingleton<IAsyncPolicy<HttpResponseMessage>>(httpRetryPolicy);

            services.AddSingleton<CircuitBreakerPolicy<HttpResponseMessage>>(breakerPolicy);
            services.AddSingleton<BulkheadPolicy<HttpResponseMessage>>(bulkheadIsolationPolicy);
            services.AddSingleton(CustomPolicyWrap());

            services.AddSingleton<PolicyHolder>(new PolicyHolder());
            services.AddSingleton<IPolicyRegistry<string>>(GetRegistry());
            services.AddMvc();
        }

        public PolicyWrap<HttpResponseMessage> CustomPolicyWrap()
        {
            int _cachedResult = 0;
            var timeoutPolicy = Policy.TimeoutAsync<HttpResponseMessage>(1, onTimeoutAsync: TimeoutDelegate);

            var httpRetryPolicy =
                Policy.HandleResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
                    .Or<TimeoutRejectedException>()
                    .RetryAsync(3, onRetry: HttpRetryPolicyDelegate);

            var httpRequestFallbackPolicy = Policy.HandleResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
                .Or<TimeoutRejectedException>()
                .FallbackAsync(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new ObjectContent(_cachedResult.GetType(), _cachedResult, new JsonMediaTypeFormatter())
                }, onFallbackAsync: HttpRequestFallbackPolicyDelegate);

            var policyWrap = Policy.WrapAsync(httpRequestFallbackPolicy, httpRetryPolicy, timeoutPolicy);
            return policyWrap;
        }

        #region PolicyWrapPoliciesDelegates
        private Task TimeoutDelegate(Context context, TimeSpan timeSpan, Task arg3)
        {
            Console.WriteLine("In OnTimeoutAsync");
            return Task.CompletedTask;
        }

        private void HttpRetryPolicyDelegate(DelegateResult<HttpResponseMessage> delegateResult, int i)
        {
            Console.WriteLine("In HttpRetryPolicyDelegate");
        }

        private Task HttpRequestFallbackPolicyDelegate(DelegateResult<HttpResponseMessage> delegateResult, Context context)
        {
            Console.WriteLine("In OnFallbackAsync");
            return Task.CompletedTask;
        } 
        #endregion

        #region CircuitBreakerDelegates
        private void OnHalfOpen()
        {
            Console.WriteLine("Connection half open");
        }

        private void OnReset(Context context)
        {
            Console.WriteLine("Connection reset");
        }

        private void OnBreak(DelegateResult<HttpResponseMessage> delegateResult, TimeSpan timeSpan, Context context)
        {
            //Console.WriteLine($"Connection break: {delegateResult.Result}, {delegateResult.Result}");
            Console.WriteLine($"Connection break");
        }
        #endregion

        #region BulkheadDelegates
        private Task OnBulkheadRejectedAsync(Context context)
        {
            Debug.WriteLine($"PollyDemo OnBulkheadRejectedAsync Executed");
            return Task.CompletedTask;
        }
        #endregion

        private IPolicyRegistry<string> GetRegistry()
        {
            IPolicyRegistry<string> registry = new PolicyRegistry();

            IAsyncPolicy<HttpResponseMessage> httpRetryPolicy =
                Policy.HandleResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
                    .RetryAsync(3);

            registry.Add("SimpleHttpRetryPolicy", httpRetryPolicy);

            IAsyncPolicy httpClientTimeoutExceptionPolicy = Policy.Handle<HttpRequestException>()
                .WaitAndRetryAsync(1, retryAttempt => TimeSpan.FromSeconds(retryAttempt));

            registry.Add("SimpleHttpTimeoutPolicy", httpClientTimeoutExceptionPolicy);

            return registry;
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IMemoryCache memoryCache)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
        }
    }
}                                                       
