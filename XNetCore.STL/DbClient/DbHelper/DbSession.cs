using System;
using System.Data;
using System.Data.Common;
using System.Text;

namespace XNetCore.STL
{
    /// <summary>
    /// 数据连接
    /// </summary>
    internal class DbSession
    {
        /// <summary>
        /// 数据库连接配置
        /// </summary>
        private DbConfig dbConfig;
        public DbSession(DbConfig config)
        {
            this.dbConfig = config;
        }

        #region DbProviderFactory
        private IDbTypeProvider _dbTypeProvider;
        /// <summary>
        /// 获取数据库操作
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        private IDbTypeProvider GetDbTypeProvider(DbConfig config)
        {
            return new DbTypeProviderFactory().GetDbTypeProvider(config);
        }
        /// <summary>
        /// 获取数据库操作
        /// </summary>
        private IDbTypeProvider DbTypeProvider
        {
            get
            {
                if (_dbTypeProvider == null)
                {
                    _dbTypeProvider = GetDbTypeProvider(this.dbConfig);
                }
                return _dbTypeProvider;
            }
        }
        #endregion

        #region DbProviderFactory
        /// <summary>
        /// 获取数据库操作工厂
        /// </summary>
        public DbProviderFactory DbProviderFactory
        {
            get
            {
                return this.DbTypeProvider.DbProvider;
            }
        }
        #endregion

        #region DbConnection
        private IDbConnection _dbConnection;
        /// <summary>
        /// 获取数据库连接
        /// </summary>
        /// <returns></returns>
        private IDbConnection GetDbConnection()
        {
            var result = this.DbProviderFactory.CreateConnection();
            result.ConnectionString = this.DbTypeProvider.DbConnectionString;
            result.Open();
            return result;
        }
        /// <summary>
        /// 数据库连接
        /// </summary>
        public IDbConnection DbConnection
        {
            get
            {
                if (_dbConnection == null)
                {
                    _dbConnection = GetDbConnection();
                }
                return _dbConnection;
            }
        }
        #endregion

        #region DbCommand
        private IDbCommand _dbCommand;
        /// <summary>
        /// 创建数据库操作
        /// </summary>
        /// <returns></returns>
        private IDbCommand CreateDbCommand()
        {
            return this.DbConnection.CreateCommand();
        }
        /// <summary>
        /// 数据库操作
        /// </summary>
        public IDbCommand DbCommand
        {
            get
            {
                if (_dbCommand == null)
                {
                    _dbCommand = CreateDbCommand();
                    _dbCommand.CommandType = CommandType.Text;
                }
                return _dbCommand;
            }
        }
        #endregion

        
        #region IDbTransaction
        private IDbTransaction _dbTransaction;
        /// <summary>
        /// 数据库操作事务
        /// </summary>
        public IDbTransaction DbTransaction
        {
            get
            {
                return _dbTransaction;
            }
        }
        /// <summary>
        /// 开始数据库操作事务
        /// </summary>
        public void BeginTransaction()
        {
            if (this._dbTransaction != null)
            {
                return;
            }
            this._dbTransaction = this.DbConnection.BeginTransaction();
        }
        /// <summary>
        /// 提交数据库操作事务
        /// </summary>
        public void CommitTransaction()
        {
            if (this._dbTransaction == null)
            {
                return;
            }
            this._dbTransaction.Commit();
            this._dbTransaction = null;
        }
        /// <summary>
        /// 回滚数据库操作事务
        /// </summary>
        public void RollbackTransaction()
        {
            if (this._dbTransaction == null)
            {
                return;
            }
            this._dbTransaction.Rollback();
            this._dbTransaction = null;
        }
        #endregion

        /// <summary>
        /// 关闭数据库
        /// </summary>
        public void Close()
        {
            this.CommitTransaction();
            this.DbConnection.Close();
            this.DbConnection.Dispose();
            this._dbConnection = null;
        }

    }
}
