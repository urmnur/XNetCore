using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace XNetCore.STL
{
    public class XContext
    {
        #region 单例模式
        private static object lockobject = new object();
        /// <summary>
        /// 当前线程上下文
        /// </summary>
        public static XContextData Current
        {
            get
            {
                if (local.Value == null)
                {
                    lock (lockobject)
                    {
                        if (local.Value == null)
                        {
                            local.Value = new XContextData();
                        }
                    }
                }
                return local.Value;
            }
            set
            {
                local.Value = value;
            }
        }
        private XContext()
        {
            InitializeTraceInfo();
        }
        /// <summary>
        /// 初始化
        /// </summary>
        private void InitializeTraceInfo()
        {
            local = new AsyncLocal<XContextData>();
        }
        private static AsyncLocal<XContextData> local = new AsyncLocal<XContextData>();
        #endregion
    }
}
