using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Newtonsoft.Json.Linq;

namespace XNetCore.STL
{
    /// <summary>
    /// 配置缓存类
    /// </summary>
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


        private string getExtension(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                return string.Empty;
            }
            var idx = fileName.LastIndexOf('.');
            if (idx>=0)
            {
                return fileName.Substring(idx).ToLower();
            }
            return string.Empty;
        }

        private XConfig getXConfig(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                return new SqliteConfigClient(fileName);
            }
            var extension = getExtension(fileName);
            if (extension==".json")
            {
                return new JsonConfigClient(fileName);
            }
            return new SqliteConfigClient(fileName);
        }
       
        /// <summary>
        /// 获取缓存信息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public Config<JObject> GetConfig(string fileName,string key)
        {
            var configService = getXConfig(fileName);
            return configService.GetConfig<JObject>(key);
        }
        /// <summary>
        /// 设置缓存信息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="cache"></param>
        /// <returns></returns>
        public int SetConfig(string fileName, Config<JObject> config)
        {
            var configService = getXConfig(fileName);
            return configService.SetConfig<JObject>(config);
        }
    }

}
