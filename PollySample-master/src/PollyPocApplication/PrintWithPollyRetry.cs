using System;
using System.Collections.Generic;

using Polly;
using Polly.Retry;

namespace PollyPocApplication
{
    public class PrintWithPollyRetry
    {
        private RetryPolicy retryPolicy = null;
        public PrintWithPollyRetry()
        {
            int retries = 3;
            retryPolicy = Policy
             .Handle<Exception>()
             .Or<DivideByZeroException>()
             .Retry(retries, (ex, count, context) =>
             {
                 Console.WriteLine($"Retrying..{count}");
             });
        }
        public void Print()
        {
            try
            {
                retryPolicy.Execute((context) =>
                    {
                        int a = 10;
                        int j = a / 0;
                    }, new Context("Print with Plly N Retry", new Dictionary<string, object> { { "a", 10 } }));
            }
            catch (Exception)
            {
                Console.WriteLine("Finally Throw the exception");
            }
        }
    }
}
