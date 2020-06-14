using System;
using System.Collections.Generic;

using Polly;
using Polly.Fallback;

namespace PollyPocApplication
{
    public class PrintPollyFallback
    {
        private FallbackPolicy fallbackPolicy;
        public PrintPollyFallback()
        {
            fallbackPolicy = Policy
                .Handle<Exception>()
                .Or<DivideByZeroException>()
                .Fallback(() =>
                {
                    Console.WriteLine("Processing Done");
                }, onFallback: (exception) =>
                {
                    Console.WriteLine(exception.Message);
                });
        }
        public void Print()
        {
            try
            {
                fallbackPolicy.Execute((context) =>
                    {
                        int a = 10;
                        int j = a / 0;
                    }, new Context("Print with Polly Fallback", new Dictionary<string, object> { { "a", 10 } }));
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
