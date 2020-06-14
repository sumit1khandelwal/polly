using System;

using Polly;
using Polly.Retry;

namespace PollyPocApplication
{
    public class PrintWithPollyWaitNRetry
    {
        private RetryPolicy waitNRetryPolicy = null;
        public PrintWithPollyWaitNRetry()
        {
            int retries = 5;
            waitNRetryPolicy = Policy
             .Handle<Exception>()
             .WaitAndRetry(
             retries,
             retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))
             , (ex, span, retryCount, context) =>
             {
                 Console.WriteLine($"Retrying..{retryCount} at {span}");
             });
        }
        public void Print()
        {
            try
            {
                waitNRetryPolicy.Execute(() =>
                {
                    int a = 10;
                    int j = a / 0;
                });
            }
            catch (Exception)
            {
                Console.WriteLine("Finally Throw the exception");
            }

        }
    }
}
