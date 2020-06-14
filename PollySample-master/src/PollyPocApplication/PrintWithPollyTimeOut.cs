using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Polly;
using Polly.Timeout;

namespace PollyPocApplication
{
    public class LoopAndPrintWithPollyTimeout
    {
        private TimeoutPolicy timeoutPolicy;
        public LoopAndPrintWithPollyTimeout()
        {
            timeoutPolicy = Policy
            .Timeout(1, TimeoutStrategy.Pessimistic, onTimeout:
         (context, span, t1, exception) =>
         {
             Console.WriteLine(exception.Message);
         });
        }
        public void Print()
        {
            try
            {
                timeoutPolicy.Execute((context) =>
                        {
                            Task t = Task.Run(async () =>
                              {
                                  await Task.Delay(10000);
                              });
                            t.Wait();
                        }, new Context("Print with Plly N Timeout", new Dictionary<string, object> { { "a", 10 } }));
            }
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
            catch (TimeoutException ex)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
            {

            }
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
            catch (Exception ex)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
            {
            }
        }
    }
}
