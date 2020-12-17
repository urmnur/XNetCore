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
    class ApiResponse
    {
        public int Code { get; set; }
        public string Msg { get; set; }
        public object Data { get; set; }
    }

}
