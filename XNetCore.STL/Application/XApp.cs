using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace XNetCore.STL
{
    public class XApp
    {
        #region 单例模式
        private static readonly object lockobject = new object();
        private static XAppData _instance = null;
        public static XAppData Current
        {
            get
            {
                if (_instance == null)
                {
                    lock (lockobject)
                    {
                        if (_instance == null)
                        {
                            _instance = new XAppData();
                        }
                    }

                }
                return _instance;
            }
        }
        private XApp()
        {
        }
        #endregion
    }
}
