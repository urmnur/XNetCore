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
    class JsonConfigClient : XConfig
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public JsonConfigClient(string fileName)
        {
            var fileInfo = getConfigFile(fileName);
            this.ConfigFile = fileInfo;
        }
        private FileInfo getConfigFile(string fileName)
        {
            const string CONFIG_FILE_NAME = "XNetCore.Config";
            if (string.IsNullOrWhiteSpace(fileName) || fileName.Equals(".json", StringComparison.OrdinalIgnoreCase))
            {
                fileName = CONFIG_FILE_NAME + ".json";
            }
            var st = new StackTrace();
            var sfs = st.GetFrames();
            var method = sfs[2].GetMethod();
            var type = method.ReflectedType;
            var fi = new FileInfo(new Uri(type.Assembly.CodeBase).LocalPath);
            var path = Path.Combine(fi.Directory.FullName, fileName);
            var result = new FileInfo(path);
            if (result.Exists)
            {
                return result;
            }
            path = Path.Combine(fi.Directory.FullName, CONFIG_FILE_NAME, fileName);
            result = new FileInfo(path);
            if (result.Exists)
            {
                return result;
            }
            var pfile = LocalPath.ProcessFile;
            var dir = new DirectoryInfo(Path.Combine(pfile.Directory.FullName, CONFIG_FILE_NAME));
            if (!dir.Exists)
            {
                dir.Create();
            }
            path = Path.Combine(dir.FullName, fileName);
            result = new FileInfo(path);
            return result;
        }

        /// <summary>
        /// 配置文件
        /// </summary>
        public FileInfo ConfigFile { get; private set; }

        /// <summary>
        /// 获取缓存信息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public Config<T> GetConfig<T>(string key)
        {
            T value = default(T);
            var file = this.ConfigFile;
            if (file.Exists)
            {
                if (string.IsNullOrWhiteSpace(key))
                {
                    value = getAllConfig<T>();
                }
                else
                {
                    value = getConfigByKey<T>(key);
                }
            }
            var result = new Config<T>();
            result.Key = key;
            result.Value = value;
            result.ExpireTime = DateTime.MaxValue;
            result.RefreshTime = DateTime.Now;
            result.CreateTime = DateTime.Now;
            return result;
        }

        private T getConfigByKey<T>(string key)
        {
            T result = default(T);
            var file = this.ConfigFile;
            var configJson = File.ReadAllText(file.FullName);
            if (string.IsNullOrWhiteSpace(configJson))
            {
                return result;
            }
            var jObject = configJson.ToJObject();
            if (jObject == null)
            {
                return result;
            }
            var jToken = jObject[key];
            if (jToken == null)
            {
                return result;
            }
            result = jToken.ToJson().ToObject<T>();
            return result;
        }
        private T getAllConfig<T>()
        {
            var file = this.ConfigFile;
            var configJson = File.ReadAllText(file.FullName);
            if (string.IsNullOrWhiteSpace(configJson))
            {
                return default(T);
            }
            return configJson.ToObject<T>();
        }

        /// <summary>
        /// 设置缓存信息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        public int SetConfig<T>(Config<T> config)
        {
            if (config == null)
            {
                return 0;
            }
            if (string.IsNullOrWhiteSpace(config.Key))
            {
                return setAllConfig<T>(config.Value);
            }
            else
            {
                return setConfigByKey(config);
            }
        }
        /// <summary>
        /// 设置缓存信息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        private int setConfigByKey<T>(Config<T> config)
        {
            if (config == null)
            {
                return 0;
            }
            var jObject = new JObject();
            var file = this.ConfigFile;
            if (file.Exists)
            {
                jObject = File.ReadAllText(file.FullName).ToJObject();
            }
            var jToken = JToken.FromObject(config.Value);
            if (jObject.ContainsKey(config.Key))
            {
                jObject[config.Key] = jToken;
            }
            else
            {
                jObject.Add(config.Key, jToken);
            }
            File.WriteAllText(file.FullName, jObject.ToIndentedJson());
            return 1;
        }

        private int setAllConfig<T>(T config)
        {
            if (config == null)
            {
                return 0;
            }
            var file = this.ConfigFile;
            File.WriteAllText(file.FullName, config.ToIndentedJson());
            return 1;
        }
    }

}
