using System;

namespace XNetCore.App
{
    static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            XNetCore.Runner.RunApp.AppMain(args);
        }
    }
}
