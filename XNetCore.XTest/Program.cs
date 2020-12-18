using System;

namespace XNetCore.XTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello XNetCore XTest!");
            try
            {
                xtest();
                Console.WriteLine("XTest Finish");
            }
            catch(Exception ex)
            {
                Console.WriteLine("XTest Error\r\n" + ex.ToString());
            }
            Console.ReadLine();
        }
        static void xtest()
        {
            new TXApiClient().MyTest();
        }
    }
}
