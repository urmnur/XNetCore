using System;
namespace XNetCore.STL
{
    /// <summary>
    /// 数据分页
    /// </summary>
    public class DbPage
    {
        /// <summary>
        /// 第几页
        /// </summary>
        public int PageIndex { get; set; }
        /// <summary>
        /// 每页条数
        /// </summary>
        public int PageRowCount { get; set; }
        /// <summary>
        /// 总条数
        /// </summary>
        public int TotalRowCount { get; set; }
    }
}
