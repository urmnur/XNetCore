using System;
using System.IO;
using System.Reflection;
using System.Text;
using XNetCore.STL;

namespace XNetCore.STL
{
    /// <summary>
    /// Log4Net日志
    /// </summary>
    class XLog4net
    {
        #region 单例模式
        private static object lockobject = new object();
        private static XLog4net _instance = null;
        public static XLog4net Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (lockobject)
                    {
                        if (_instance == null)
                        {
                            _instance = new XLog4net();
                        }
                    }

                }
                return _instance;
            }
        }
        private XLog4net()
        {
            try
            {
                log4net = new Log4net();
            }
            catch { }
        }
        private Log4net log4net;
        #endregion
        public int Write(Log log)
        {
            if (log==null)
            {
                return 0;
            }
            return doWrite(log);
        }



        private int doWrite(Log log)
        {
            if (log4net==null)
            {
                return 0;
            }
            var msg = log.ToString();
            switch (log.Level)
            {
                case LogLevel.Trace:
                    {
                        log4net.Trace(msg);
                        break;
                    }
                case LogLevel.Debug:
                    {
                        log4net.Debug(msg);
                        break;
                    }
                case LogLevel.Info:
                    {
                        log4net.Info(msg);
                        break;
                    }
                case LogLevel.Warn:
                    {
                        log4net.Warn(msg);
                        break;
                    }
                case LogLevel.Error:
                    {
                        log4net.Error(msg);
                        break;
                    }
                case LogLevel.Fatal:
                    {
                        log4net.Fatal(msg);
                        break;
                    }
                default:
                    {
                        log4net.Debug(msg);
                        break;
                    }
            }
            return 1;
        }

    }


}
