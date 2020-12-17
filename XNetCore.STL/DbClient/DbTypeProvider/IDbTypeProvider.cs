using System;
using System.Data.Common;

namespace XNetCore.STL
{
    /// <summary>
    /// 数据操作类型
    /// </summary>
    interface IDbTypeProvider
    {
        /// <summary>
        /// 数据库操作工厂
        /// </summary>
        DbProviderFactory DbProvider { get; }
        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        string DbConnectionString{ get; }
    }
    /// <summary>
    /// 数据库操作类工厂
    /// </summary>
    class DbTypeProviderFactory
    {
        /// <summary>
        /// 获取数据库操作类
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public IDbTypeProvider GetDbTypeProvider(DbConfig config)
        {
            var types = this.GetType().Assembly.GetTypes();
            Type dbType = null;
            foreach(var type in types)
            {
                if (type.FullName.ToLower()==("XNetCore.STL."+config.DbType+"Provider").ToLower())
                {
                    dbType = type;
                    break;
                }
            }
            if (dbType==null)
            {
                throw new Exception("数据库类型不正确");
            }
            object[] parms = new object[] { config};
            var result = Activator.CreateInstance(dbType, parms);
            return result as IDbTypeProvider;
        }
    }

}
