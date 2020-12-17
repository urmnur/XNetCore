using System;

namespace XNetCore.XAPI.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            XNetCore.XAPI.XApi.Run(9876);
            Console.ReadLine();
        }
    }
}
