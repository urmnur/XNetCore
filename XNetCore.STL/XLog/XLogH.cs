using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using XNetCore.STL;

namespace XNetCore.STL
{
    /// <summary>
    /// Log4Net日志
    /// </summary>
    class XLogH
    {
        #region 单例模式
        private static object lockobject = new object();
        private static XLogH _instance = null;
        public static XLogH Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (lockobject)
                    {
                        if (_instance == null)
                        {
                            _instance = new XLogH();
                        }
                    }

                }
                return _instance;
            }
        }
        private XLogH()
        {
        }
        #endregion
        public int Write(Log log)
        {
            if (log == null)
            {
                return 0;
            }
            iniLogMethodName(log);
            return XLog4net.Instance.Write(log);
        }

        private void iniLogMethodName(Log log)
        {
            if (string.IsNullOrWhiteSpace(log.Method))
            {
                log.Method = getLogMethodName(0);
                return;
            }

            var idx = log.Method.ToInt(0);
            if (idx > 0)
            {
                log.Method = getLogMethodName(idx);
            }
        }

        private string getLogMethodName(int idx)
        {
            var frameIndex = 3 + idx;
            var result = string.Empty;
            try
            {
                var st = new StackTrace(true);
                var sf = st.GetFrame(frameIndex);
                var method = sf.GetMethod();

                result = method.DeclaringType.FullName + "." + method.Name;
            }
            catch { }
            return result;
        }
    }


}
