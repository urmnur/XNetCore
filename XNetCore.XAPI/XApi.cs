using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using XNetCore.STL;

namespace XNetCore.XAPI
{
    public  class XApi
    {
        public  void Start(int port)
        {
            var url = $"http://*:{port}/";
            Task.Run(()=>{ runWebApiServer(url); });
        }
        private  void runWebApiServer(string url)
        {
            try
            {
                WebHost.CreateDefaultBuilder()
                .UseUrls(url)
                .UseStartup<Startup>()
                .Build()
                .Run();
            }
            catch (Exception ex)
            {
                throw new System.Exception($"APIÆô¶¯Ê§°Ü>>{url}", ex);
            }
        }
    }
}
