using System;
namespace XNetCore.STL
{
    /// <summary>
    /// 数据库配置类
    /// </summary>
    public class DbConfig
    {
        /// <summary>
        /// 数据库名称
        /// </summary>
        public string CfgName { get; set; }
        /// <summary>
        /// 数据库类型
        /// </summary>
        public string DbType { get; set; }
        /// <summary>
        /// 数据库IP
        /// </summary>
        public string DbHost { get; set; }
        /// <summary>
        /// 数据库端口
        /// </summary>
        public int DbPort { get; set; }
        /// <summary>
        /// 数据库用户名
        /// </summary>
        public string DbUser { get; set; }
        /// <summary>
        /// 数据库密码
        /// </summary>
        public string DbPwd { get; set; }
        /// <summary>
        /// 数据库名称
        /// </summary>
        public string DbName { get; set; }
    }
}
