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
    class SqliteConfigClient : XConfig
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public SqliteConfigClient(string fileName)
        {
            var configFile = getConfigFile(fileName);
            if (!configFile.Exists)
            {
                return;
            }
            createTable(configFile);
            this.ConfigFile = configFile;
        }
        private FileInfo getConfigFile(string fileName)
        {
            const string CONFIG_FILE_NAME = "XNetCore.Config";
            if (string.IsNullOrWhiteSpace(fileName))
            {
                fileName = CONFIG_FILE_NAME + ".db";
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
        /// 获取缓存文件路径
        /// </summary>
        /// <returns></returns>
        private DbConfig getDbConfig(FileInfo configFile)
        {
            if (configFile == null)
            {
                return null;
            }
            if (!configFile.Exists)
            {
                return null;
            }
            var dbConfig = new DbConfig();
            dbConfig.DbType = "sqlite";
            dbConfig.DbName = configFile.FullName;
            return dbConfig;
        }

        private void createTable(FileInfo configFile)
        {
            if (configFile == null)
            {
                return;
            }
            if (!configFile.Exists)
            {
                return;
            }
            var dbConfig = getDbConfig(configFile);
            if (dbConfig == null)
            {
                return;
            }
            var dbHelper = new DbHelper(dbConfig);
            dbHelper.CommandText = @"CREATE TABLE IF NOT EXISTS SYS_CONFIG(
                   CONF_KEY VARCHAR(500) PRIMARY KEY,
                   CONF_VALUE VARCHAR(500),
                   EXPIRE_TIME TIMESTAMP default (DATETIME('NOW', 'LOCALTIME')),
                   CREATE_TIME TIMESTAMP default (DATETIME('NOW', 'LOCALTIME')),
                   MODIFY_TIME TIMESTAMP default (DATETIME('NOW', 'LOCALTIME'))
                ); ";
            dbHelper.ExecuteNonQuery();
        }
        /// <summary>
        /// 获取缓存信息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public Config<T> GetConfig<T>(string key)
        {
            var file = this.ConfigFile;
            if (file.Exists)
            {
                if (string.IsNullOrWhiteSpace(key))
                {
                    return getAllConfig<T>();
                }
                else
                {
                    return getConfigByKey<T>(key);
                }
            }
            var result = new Config<T>();
            result.Key = key;
            result.Value = default(T);
            result.ExpireTime = DateTime.MaxValue;
            result.RefreshTime = DateTime.Now;
            result.CreateTime = DateTime.Now;
            return result;
        }
        private Config<T> getConfigByKey<T>(string key)
        {
            var dbConfig = getDbConfig(this.ConfigFile);
            if (dbConfig == null)
            {
                return null;
            }
            var dbHelper = new DbHelper(dbConfig);
            dbHelper.CommandText = "SELECT * FROM SYS_CONFIG WHERE CONF_KEY = @CONF_KEY;";
            dbHelper.AddParameter("CONF_KEY", key);
            var dTable = dbHelper.GetDataTable();
            if (dTable == null || dTable.Rows.Count == 0)
            {
                return null;
            }
            var row = dTable.Rows[0];
            var result = new Config<T>();
            result.Key = row.Value("CONF_KEY");
            result.ExpireTime = row.Value("EXPIRE_TIME").ToDateTime(DateTime.Now);
            result.CreateTime = row.Value("CREATE_TIME").ToDateTime(DateTime.Now);
            result.RefreshTime = row.Value("MODIFY_TIME").ToDateTime(DateTime.Now);
            var value = row.Value("CONF_VALUE");
            result.Value = value.ToObject<T>();
            return result;
        }
        private Config<T> getAllConfig<T>()
        {
            return getConfigByKey<T>("DEFAULT_CONFIG");
        }
        /// <summary>
        /// 设置缓存信息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="cache"></param>
        /// <returns></returns>
        public int SetConfig<T>(Config<T> config)
        {
            if (config == null)
            {
                return 0;
            }
            if (string.IsNullOrWhiteSpace(config.Key))
            {
                return setAllConfig<T>(config);
            }
            else
            {
                return setConfigByKey(config);
            }
        }
        private int setConfigByKey<T>(Config<T> config)
        {
            var dbConfig = getDbConfig(this.ConfigFile);
            if (dbConfig == null)
            {
                return -1;
            }
            var key = config.Key;
            var str = config.Value.ToJson();
            var dbHelper = new DbHelper(dbConfig);
            dbHelper.CommandText = "DELETE FROM SYS_CONFIG WHERE CONF_KEY = @CONF_KEY;";
            dbHelper.ClearParameters();
            dbHelper.AddParameter("CONF_KEY", key);
            dbHelper.ExecuteNonQuery();

            dbHelper.CommandText = "REPLACE INTO SYS_CONFIG (CONF_KEY, CONF_VALUE, EXPIRE_TIME, MODIFY_TIME) VALUES (@CONF_KEY, @CONF_VALUE, @EXPIRE_TIME, @MODIFY_TIME);";
            dbHelper.ClearParameters();
            dbHelper.AddParameter("CONF_KEY", key);
            dbHelper.AddParameter("EXPIRE_TIME", config.ExpireTime);
            dbHelper.AddParameter("CONF_VALUE", str);
            dbHelper.AddParameter("MODIFY_TIME", DateTime.Now);
            return dbHelper.ExecuteNonQuery();
        }


        private int setAllConfig<T>(Config<T> config)
        {
            config.Key = "DEFAULT_CONFIG";
            return SetConfig<T>(config);
        }

    }
}
