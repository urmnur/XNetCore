using System;

namespace XNetCore.XTest
{
    class Program
    {
        static void Main(string[] args)
        {
            XNetCore.STL.Runner.Instance.AppendPrivatePaths();
            Console.WriteLine("Hello XNetCore XTest!");
            try
            {
                xtest();
                Console.WriteLine("XNetCore XTest Finish");
            }
            catch(Exception ex)
            {
                Console.WriteLine("XNetCore XTest Error\r\n" + ex.ToString());
            }
            Console.ReadLine();
        }
        static void xtest()
        {
            new TestXApiClient().Test();
        }
    }
}
