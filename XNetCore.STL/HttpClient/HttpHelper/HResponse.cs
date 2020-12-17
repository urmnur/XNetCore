using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace XNetCore.STL
{
    /// <summary>
    /// Http 返回值
    /// </summary>
    public class HResponse
    {
        /// <summary>
        /// 返回类型
        /// </summary>
        public HttpStatusCode StatusCode { get; set; }
        /// <summary>
        /// 返回数据
        /// </summary>
        public string ResponseData { get; set; }
    }

    /// <summary>
    /// 键值
    /// </summary>
    public class HForm
    {
        /// <summary>
        /// 键值对
        /// </summary>
        public string Key { get; set; }
        /// <summary>
        /// 值
        /// </summary>
        public string Value { get; set; }
    }
}
