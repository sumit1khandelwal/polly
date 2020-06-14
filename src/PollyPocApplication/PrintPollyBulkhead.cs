using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Polly;
using Polly.Bulkhead;

namespace PollyPocApplication
{
    public class PrintPollyBulkhead
    {
        private BulkheadPolicy bulkheadPolicy;
        public PrintPollyBulkhead()
        {
            bulkheadPolicy = Policy.Bulkhead(2, 2, onBulkheadRejected: (context) =>
            {
                Console.WriteLine(context.Count);
            });
        }
        public void Print()
        {
            try
            {
                List<Task> tasks = new List<Task>();
                for (int i = 0 ; i < 5 ; i++)
                {
                    bulkheadPolicy.Execute((context) =>
                            {
                                Task t = Task.Run(async () =>
                                {
                                    await Task.Delay(100);
                                });
                                tasks.Add(t);

                            }, new Context("Print with Plly N Retry", new Dictionary<string, object> { { "a", 10 } }));
                }
                Task.WaitAll(tasks.ToArray());
            }
            catch (Exception)
            {
                Console.WriteLine("Finally Throw the exception");
            }
        }
    }
}
