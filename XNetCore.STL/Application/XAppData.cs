using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace XNetCore.STL
{

    public class XAppData
    {
        internal XAppData()
        {
            this.AppPath = getAppPath();
            this.Localhost = "127.0.0.1";
            this.AppId = "XNetCore";
            this.DbConfig = "";
        }

        private string getAppPath()
        {
            var assembly = Assembly.GetEntryAssembly();
            if (assembly == null)
            {
                assembly = typeof(LocalPath).Assembly;
            }
            var dir = new FileInfo(new Uri(assembly.CodeBase).LocalPath).Directory;
            if (dir.Name.ToLower() == "bin")
            {
                dir = dir.Parent;
            }
            return dir.FullName;
        }
        
        public string AppPath { get; private set; }
        public string AppId { get; set; }
        public string Localhost { get; set; }
        public string DbConfig { get; set; }
    }
}
