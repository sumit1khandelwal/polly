using Polly;
using Polly.Retry;
using System;
using System.Net.Http;


namespace PollyPocAPIApplication
{
    public class PolicyHolder
    {
        public RetryPolicy<HttpResponseMessage> HttpRetryPolicy { get; private set; }
        public RetryPolicy HttpClientTimeoutException { get; private set; }

        public PolicyHolder()
        {
            HttpRetryPolicy =
               Policy.HandleResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
               .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(retryAttempt), (response, timespan) =>
               {
                   var result = response.Result;
                   // log the result
               });

            HttpClientTimeoutException = Policy.Handle<HttpRequestException>()
                .WaitAndRetryAsync(1, retryAttempt => TimeSpan.FromSeconds(retryAttempt),
                    onRetry: (exception, timespan) =>
                    {
                        string message = exception.Message;
                        // log the message.
                    }
                );
        }
    }
}
