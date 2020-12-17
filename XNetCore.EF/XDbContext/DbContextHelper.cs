using System;
using Microsoft.EntityFrameworkCore;
using XNetCore.STL;

namespace XNetCore.EF
{
    class DbContextHelper
    {
        public void OnConfiguring(DbContextOptionsBuilder options, DbConfig config)
        {
            var constr = getDbConnectionString(config);
            var extensio = getDbContexExtension(config);
            extensio.UseDB(options, constr);
        }




        private string getDbConnectionString(DbConfig config)
        {
            var provider = ("XNetCore.STL." + config.DbType + "Provider, XNetCore.STL").ToType().Instance(new object[] { config });
            var p = provider.GetType().GetProperty("DbConnectionString");
            var result = p.GetValue(provider).ToString();
            return result;
        }

        private DbContexExtension getDbContexExtension(DbConfig config)
        {
            var types = this.GetType().Assembly.GetTypes();
            Type dbType = null;
            foreach (var type in types)
            {
                if (type.FullName.ToLower() == ($"{this.GetType().Namespace}.{config.DbType }Extension").ToLower())
                {
                    dbType = type;
                    break;
                }
            }
            if (dbType == null)
            {
                throw new Exception("数据库类型不正确");
            }
            var result = Activator.CreateInstance(dbType);
            return result as DbContexExtension;
        }

    }

    interface DbContexExtension
    {
        void UseDB(DbContextOptionsBuilder options, string constr);
    }

}
