using System;
using System.Data.Common;
using System.IO;
using System.Reflection;
using System.Text;

namespace XNetCore.STL
{
    /// <summary>
    /// Sqlite数据库操作
    /// </summary>
    internal class SqliteProvider : IDbTypeProvider
    {

        private DbConfig dbConfig;
        public SqliteProvider(DbConfig config)
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
            var type = Type.GetType("System.Data.SQLite.Linq.SQLiteProviderFactory, System.Data.SQLite.Linq", true, true);
            var result = type.Assembly.CreateInstance(type.FullName) as DbProviderFactory;
            return result;
        }


        #region DbProviderFactory
        private string _dbConnectionString;
        private string GetDbConnectionString(DbConfig config)
        {
            var result = new StringBuilder();
            if (!string.IsNullOrWhiteSpace(config.DbName))
            {
                result.Append("Data Source = ").Append(config.DbName).Append(";");
            }
            else if(!string.IsNullOrWhiteSpace(config.DbHost))
            {
                result.Append("Data Source = ").Append(config.DbHost).Append(";");
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
