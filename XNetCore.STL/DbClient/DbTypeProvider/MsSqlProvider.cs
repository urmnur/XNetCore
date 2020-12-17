using System;
using System.Data.Common;
using System.Text;

namespace XNetCore.STL
{
    /// <summary>
    /// MsSql数据库操作
    /// </summary>
    internal class MsSqlProvider : IDbTypeProvider
    {

        private DbConfig dbConfig;
        public MsSqlProvider(DbConfig config)
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

        private DbProviderFactory getDbProviderFactory()
        {
            var type = Type.GetType("System.Data.SqlClient.SqlClientFactory, System.Data", true, true);
            var p = type.GetField("Instance");
            if (p == null)
            {
                return null;
            }
            var obj = p.GetValue(null);
            if (obj == null)
            {
                return null;
            }
            var result = obj as DbProviderFactory;
            return result;
        }

        #endregion


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
                result.Append("User = ").Append(config.DbUser).Append(";");
            }
            if (!string.IsNullOrWhiteSpace(config.DbPwd))
            {
                result.Append("Password = ").Append(config.DbPwd).Append(";");
            }
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
