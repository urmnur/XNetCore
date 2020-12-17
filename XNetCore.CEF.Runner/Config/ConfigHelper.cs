using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XNetCore.STL;

namespace XNetCore.CEF.Runner
{
    class ConfigHelper
    {
        #region 单例模式
        private static object lockobject = new object();
        private static ConfigHelper _instance = null;
        public static ConfigHelper Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (lockobject)
                    {
                        if (_instance == null)
                        {
                            _instance = new ConfigHelper();
                        }
                    }

                }
                return _instance;
            }
        }
        private ConfigHelper()
        {
        }
        #endregion

        private XConfig getConfig()
        {
            var result = new ConfigClient(".json");
            return result;
        }

        public AppDomain AppDomain
        {
            get
            {
                var result = getConfig().GetConfig<AppDomain>("AppDomain").Value;
                if (result == null)
                {
                    return new AppDomain();
                }
                return result;
            }
        }
    }

    class AppDomain
    {
        public string Url { get; set; }
        public int ApiPort { get; set; }
        public int RpcPort { get; set; }
        public bool ShowNotify { get; set; }
        public string JsClose { get; set; }
        public bool Maximized { get; set; }
    }
}
