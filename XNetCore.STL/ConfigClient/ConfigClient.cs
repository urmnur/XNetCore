using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace XNetCore.STL
{
    /// <summary>
    /// 配置缓存类
    /// </summary>
    public class ConfigClient: XConfig
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public ConfigClient(string fileName)
        {
            this.configService = getXConfig(fileName);
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        public ConfigClient():this(string.Empty)
        {
        }

        private XConfig configService;


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
        /// 配置文件
        /// </summary>
        public FileInfo ConfigFile { get { return configService.ConfigFile; } }
       
        /// <summary>
        /// 获取缓存信息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public Config<T> GetConfig<T>(string key)
        {
            return configService.GetConfig<T>(key);
        }
        /// <summary>
        /// 设置缓存信息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="cache"></param>
        /// <returns></returns>
        public int SetConfig<T>(Config<T> cache)
        {
            return configService.SetConfig<T>(cache);
        }

    }

}
