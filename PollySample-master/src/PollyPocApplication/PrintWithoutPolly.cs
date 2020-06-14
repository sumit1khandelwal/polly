using System;

namespace PollyPocApplication
{
    public class PrintWithoutPolly
    {
        public void Print()
        {
            bool error = false;
            int retries = 3;
            while (true)
            {
                try
                {
                    int a = 10;
                    int j = a / 0;
                    break; // success!
                }
                catch
                {
                    Console.WriteLine("Catch");
                    if (--retries == 0)
                    {
                        error = true;
                        break;
                    }
                }
            }

            if (error)
            {
                Console.WriteLine("Some error occured");
            }
        }
    }
}
