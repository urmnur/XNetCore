using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XNetCore.STL
{
    public interface XConfig
    {
        /// <summary>
        /// 配置文件
        /// </summary>
        FileInfo ConfigFile { get; }
        /// <summary>
        /// 获取缓存信息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        Config<T> GetConfig<T>(string key);
        /// <summary>
        /// 设置缓存信息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="cache"></param>
        /// <returns></returns>
        int SetConfig<T>(Config<T> config);
    }


    /// <summary>
    /// 缓存信息
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Config<T>
    {
        public Config()
        {
            this.CreateTime = DateTime.Now;
            this.RefreshTime = this.CreateTime;
            this.ExpireTime = DateTime.MaxValue;
        }
        /// <summary>
        /// 配置名称
        /// </summary>
        public string Key { get; set; }
        /// <summary>
        /// 配置信息
        /// </summary>
        public T Value { get; set; }
        /// <summary>
        /// 超时时间
        /// </summary>
        public DateTime ExpireTime { get; set; }
        /// <summary>
        /// 刷新时间
        /// </summary>
        public DateTime RefreshTime { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
    }

}
