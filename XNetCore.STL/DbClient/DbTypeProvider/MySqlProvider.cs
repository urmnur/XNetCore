using System;
using System.Data.Common;
using System.IO;
using System.Reflection;
using System.Text;

namespace XNetCore.STL
{
    /// <summary>
    /// MySql数据库操作
    /// </summary>
    internal class MySqlProvider : IDbTypeProvider
    {

        private DbConfig dbConfig;
        public MySqlProvider(DbConfig config)
        {
            this.dbConfig = config;
        }

        #region DbProviderFactory
        private DbProviderFactory _dbProvider;
        public DbProviderFactory DbProvider
        {
            get
            {
                if (_dbProvider == null)
                {
                    _dbProvider = getDbProviderFactory();
                }
                return _dbProvider;
            }
        }
        #endregion


        private DbProviderFactory getDbProviderFactory()
        {
            var type = Type.GetType("MySql.Data.MySqlClient.MySqlClientFactory, MySql.Data", true, true);
            var result = type.Assembly.CreateInstance(type.FullName) as DbProviderFactory;
            return result;
        }


        #region DbProviderFactory
        private string _dbConnectionString;
        private string GetDbConnectionString(DbConfig config)
        {

            var result = new StringBuilder();
            if (!string.IsNullOrWhiteSpace(config.DbHost))
            {
                result.Append("Server = ").Append(config.DbHost).Append(";");
            }
            if (config.DbPort > 0)
            {
                result.Append("Port = ").Append(config.DbPort.ToString()).Append(";");
            }
            if (!string.IsNullOrWhiteSpace(config.DbName))
            {
                result.Append("Database = ").Append(config.DbName).Append(";");
            }
            if (!string.IsNullOrWhiteSpace(config.DbUser))
            {
                result.Append("Uid = ").Append(config.DbUser).Append(";");
            }
            if (!string.IsNullOrWhiteSpace(config.DbPwd))
            {
                result.Append("Pwd = ").Append(config.DbPwd).Append(";");
            }
            result.Append("SslMode = None;");
            return result.ToString();
        }

        public string DbConnectionString
        {
            get
            {
                if (_dbConnectionString == null)
                {
                    _dbConnectionString = GetDbConnectionString(this.dbConfig);
                }
                return _dbConnectionString;
            }
        }

        #endregion
    }
}
