using System;
using System.Threading.Tasks;

using Polly;
using Polly.CircuitBreaker;

namespace PollyPocApplication
{
    public class PrintWithPollyCircuitBreaker
    {
        private CircuitBreakerPolicy circuitBreakerPolicy;
        public PrintWithPollyCircuitBreaker()
        {
            circuitBreakerPolicy = Policy
              .Handle<Exception>()
             .CircuitBreaker(2, TimeSpan.FromMilliseconds(1000),
             (ex, t) =>
             {
                 Console.WriteLine("Circuit broken!");
             }, () =>
             {
                 Console.WriteLine("Circuit Reset!");

             }, () =>
             {
                 Console.WriteLine("Circuit Half Open!");
             });

        }
        public void Print()
        {
            int retries = 100;

            for (int i = 0 ; i < retries ; i++)
            {
                try
                {
                    Console.WriteLine($"Loop count starts:{i}");
                    circuitBreakerPolicy.Execute(() =>
                            {
                                Task t = Task.Run(() =>
                                {
                                    Console.WriteLine("Code Executing....");
                                    int i = 10;
                                    int a = i / 0;

                                });
                                t.Wait();
                                Console.WriteLine($"Loop executed count:{i}");
                            });
                }
                catch (Exception)
                {
                }
            }

        }
    }
}