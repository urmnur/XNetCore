using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using XNetCore.STL;

namespace XNetCore.XAPI.Client
{
    class CurrentApp
    {
        #region 单例模式
        private static object lockobject = new object();
        private static CurrentApp _instance = null;
        public static CurrentApp Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (lockobject)
                    {
                        if (_instance == null)
                        {
                            _instance = new CurrentApp();
                        }
                    }

                }
                return _instance;
            }
        }
        private CurrentApp()
        {
        }
        #endregion

        public int Port { get; set; }

        public RegistData CreateRegistData(string serviceIntfc, string methodName)
        {
            return getRegistData(serviceIntfc, methodName);
        }
        private RegistData getRegistData(string serviceIntfc, string methodName)
        {
            if (this.Port < 1000)
            {
                return null;
            }
            if (string.IsNullOrWhiteSpace(serviceIntfc))
            {
                return null;
            }
            var impl = getImpl(serviceIntfc);
            var result = new RegistData()
            {
                Address = "127.0.0.1",
                Port = this.Port,
                ServiceInfc = serviceIntfc,
                ServiceImpl = impl,
                ServiceMethod = methodName,
            };
            return result;
        }

        private string getImpl(string serviceIntfc)
        {
            var type = serviceIntfc.ToType();
            var fs = getImplFiles(type);
            foreach (var fi in fs)
            {
                try
                {
                    var ts = Runner.Instance.GetFileTypes(fi);
                    foreach (var t in ts)
                    {
                        if (!t.IsClass)
                        {
                            continue;
                        }
                        if (type.IsAssignableFrom(t))
                        {
                            return t.FullName();
                        }
                    }
                }
                catch (Exception ex)
                {

                }
            }
            return string.Empty;
        }
        private FileInfo[] getImplFiles(Type type)
        {
            var result = new List<FileInfo>();
            var thisFile = new FileInfo(type.Assembly.Location);
            var thisfilename = thisFile.Name.ToLower();
            var startName = thisfilename.Substring(0, thisfilename.LastIndexOf("."));
            var endName = thisfilename.Substring(thisfilename.LastIndexOf("."));
            foreach (var fi in Runner.Instance.PrivateFiles)
            {
                var fileName = fi.Name.ToLower();
                if (fileName.StartsWith(startName)
                    && fileName.EndsWith(endName))
                {
                    result.Add(fi);
                }
            }
            return result.ToArray();
        }

    }
}
