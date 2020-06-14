using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PollyPocApplication
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            List<(string, Action)> samples = new List<(string, Action)>()
            {
                ("For Loop And Print Without Polly: Press 0", new PrintWithoutPolly().Print),

                ("For Loop And Print With Polly Retry: Press 1", new PrintWithPollyRetry().Print),

                ("For Loop And Print With Polly with Wait N Retry: Press 2", new PrintWithPollyWaitNRetry().Print),

                ("For Polly Circuit Breaker: Press 3", new PrintWithPollyCircuitBreaker().Print),

                ("For Polly Fallback: Press 4", new PrintPollyFallback().Print) ,

                ("For Polly Timeout: Press 5", new LoopAndPrintWithPollyTimeout().Print),

                ("For Polly Bulkhead: Press 6", new PrintPollyBulkhead().Print),

                ("Exit: Press 7", ()=>Environment.Exit(0))
        };
            foreach ((string, Action) item in samples)
            {
                Console.WriteLine(item.Item1);
            }
            Console.WriteLine("Enter the number:");
            int i = 0;
            CheckNumber:
            try
            {
                if (int.TryParse(Console.ReadLine(), out i) && i < samples.Count)
                {
                    Console.WriteLine("Starting...");
                    await Task.Factory.StartNew(samples[i].Item2);
                }
                else
                {
                    Console.WriteLine("Invalid sample index provided: \"{0}\"", args[0]);
                }
                Console.WriteLine("Enter the number again:");
                goto CheckNumber;
            }
            catch (Exception ex)
            {
            }

        }
    }
}
