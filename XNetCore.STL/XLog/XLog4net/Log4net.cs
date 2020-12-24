using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;

namespace XNetCore.STL
{
    /// <summary>
    /// Log4Net日志
    /// </summary>
    class Log4net: ILog
    {
        public Log4net()
        {
            IniLogInfo();
        }
        #region IniLogInfo
        private const string configFileName = "log4net.config";
        protected void IniLogInfo()
        {
            IniLog(getFilePath(configFileName));
        }
        private FileInfo getFilePath(string fileName)
        {
            var dir = LocalPath.CurrentFile.Directory;
            var result = getChildFilePath(dir, fileName);
            if (result == null)
            {
                result = getParentFilePath(dir, fileName);
            }
            return result;
        }
        private FileInfo getChildFilePath(DirectoryInfo dir, string fileName)
        {
            foreach (var fi in dir.GetFiles())
            {
                if (fi.Name.ToLower() == fileName.ToLower())
                {
                    return fi;
                }
            }
            foreach (var cdir in dir.GetDirectories())
            {
                var result = getChildFilePath(cdir, fileName);
                if (result != null)
                {
                    return result;
                }
            }
            return null;
        }
        private FileInfo getParentFilePath(DirectoryInfo dir, string fileName)
        {
            foreach (var fi in dir.GetFiles())
            {
                if (fi.Name.ToLower() == fileName.ToLower())
                {
                    return fi;
                }
            }
            if (dir.Parent == null)
            {
                return null;
            }
            return getParentFilePath(dir.Parent, fileName);
        }

        #region 初始化Log
        private object logProvider = null;
        /// <summary>
        /// 初始化日志程序集函数
        /// </summary>
        protected void IniLog(FileInfo file)
        {
            if (file == null)
            {
                throw new FileNotFoundException(configFileName);
            }
            var methodConfigureAndWatch = getMethodConfigureAndWatch();
            methodConfigureAndWatch.Invoke(null, new object[] { file });
            var methodGetLogger = getMethodGetLogger();
            logProvider = methodGetLogger.Invoke(null, new object[] { this.GetType() });
        }

        #region 初始化日志类
        private MethodInfo getMethod(string className, string methodName, Type[] types)
        {
            Type t = Type.GetType(className, true, true);
            var result = t.GetMethod(methodName, types);
            return result;
        }
        private MethodInfo getMethodConfigureAndWatch()
        {
            var className = "log4net.Config.XmlConfigurator, log4net";
            var methodName = "ConfigureAndWatch";
            return getMethod(className, methodName, new Type[] { typeof(FileInfo) });
        }
        private MethodInfo getMethodGetLogger()
        {
            var className = "log4net.LogManager, log4net";
            var methodName = "GetLogger";
            return getMethod(className, methodName, new Type[] { typeof(Type) });
        }
        #endregion
        #endregion
        #endregion


        #region 反射调用日志类
        #region 反射调用日志方法
        private string getLogMethodName()
        {
            var frameIndex = 1;
            var result = string.Empty;
            var st = new StackTrace(true);
            var sf = st.GetFrame(frameIndex);
            var sm = sf.GetMethod();
            try
            {
                result = sm.Name;
            }
            catch { }
            return result.ToString().Trim();
        }
        private void InvokeLogMethod(string methodName, params object[] ps)
        {
            var types = new List<Type>();
            for (int i = 0; i < ps.Length; i++)
            {
                types.Add((ps[i]).GetType());
            }
            var method = this.logProvider.GetType().GetMethod(methodName, types.ToArray());
            method.Invoke(this.logProvider, ps);
        }
        protected void runLogMethod(string methodName, params object[] ps)
        {
            if (this.logProvider == null)
            {
                return;
            }
            InvokeLogMethod(methodName, ps);
        }
        #endregion

        #region 反射获取日志属性
        /// <summary>
        /// 反射获取属性值
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        private bool getPropertyValue(string propertyName)
        {
            if (this.logProvider == null)
            {
                return false;
            }
            var p = this.logProvider.GetType().GetProperty(propertyName);
            return (bool)p.GetValue(this.logProvider, null);
        }
        #endregion
        #endregion

        #region ILog
        #region ILog  属性
        public bool IsTraceEnabled
        {
            get { return getPropertyValue("IsDebugEnabled"); }
        }
        public bool IsDebugEnabled
        {
            get { return getPropertyValue("IsDebugEnabled"); }
        }

        public bool IsInfoEnabled
        {
            get { return getPropertyValue("IsInfoEnabled"); }
        }

        public bool IsWarnEnabled
        {
            get { return getPropertyValue("IsWarnEnabled"); }
        }

        public bool IsErrorEnabled
        {
            get { return getPropertyValue("IsErrorEnabled"); }
        }

        public bool IsFatalEnabled
        {
            get { return getPropertyValue("IsFatalEnabled"); }
        }
        #endregion
        #region ILog  方法
        #region Trace
        private const string TraceLevelName = "Debug";
        public void Trace(object message)
        {
            runLogMethod(TraceLevelName, message);
        }
        public void Trace(object message, Exception exception)
        {
            runLogMethod(TraceLevelName, message, exception);
        }
        public void TraceFormat(string format, params object[] args)
        {
            runLogMethod(TraceLevelName, format, args);
        }
        public void TraceFormat(string format, object arg0)
        {
            runLogMethod(TraceLevelName, format, arg0);
        }
        public void TraceFormat(string format, object arg0, object arg1)
        {
            runLogMethod(TraceLevelName, format, arg0, arg1);
        }
        public void TraceFormat(string format, object arg0, object arg1, object arg2)
        {
            runLogMethod(TraceLevelName, format, arg0, arg1, arg2);
        }
        public void TraceFormat(IFormatProvider provider, string format, params object[] args)
        {
            runLogMethod(TraceLevelName, provider, format, args);
        }
        #endregion
        #region Debug
        public void Debug(object message)
        {
            this.runLogMethod(getLogMethodName(), message);
        }
        public void Debug(object message, Exception exception)
        {
            this.runLogMethod(getLogMethodName(), message, exception);
        }
        public void DebugFormat(string format, params object[] args)
        {
            this.runLogMethod(getLogMethodName(), format, args);
        }
        public void DebugFormat(string format, object arg0)
        {
            this.runLogMethod(getLogMethodName(), format, arg0);
        }
        public void DebugFormat(string format, object arg0, object arg1)
        {
            this.runLogMethod(getLogMethodName(), format, arg0, arg1);
        }
        public void DebugFormat(string format, object arg0, object arg1, object arg2)
        {
            this.runLogMethod(getLogMethodName(), format, arg0, arg1, arg2);
        }
        public void DebugFormat(IFormatProvider provider, string format, params object[] args)
        {
            this.runLogMethod(getLogMethodName(), provider, format, args);
        }
        #endregion
        #region Info
        public void Info(object message)
        {
            this.runLogMethod(getLogMethodName(), message);
        }
        public void Info(object message, Exception exception)
        {
            this.runLogMethod(getLogMethodName(), message, exception);
        }
        public void InfoFormat(string format, params object[] args)
        {
            this.runLogMethod(getLogMethodName(), format, args);
        }
        public void InfoFormat(string format, object arg0)
        {
            this.runLogMethod(getLogMethodName(), format, arg0);
        }
        public void InfoFormat(string format, object arg0, object arg1)
        {
            this.runLogMethod(getLogMethodName(), format, arg0, arg1);
        }
        public void InfoFormat(string format, object arg0, object arg1, object arg2)
        {
            this.runLogMethod(getLogMethodName(), format, arg0, arg1, arg2);
        }
        public void InfoFormat(IFormatProvider provider, string format, params object[] args)
        {

            this.runLogMethod(getLogMethodName(), provider, format, args);
        }
        #endregion
        #region Warn
        public void Warn(object message)
        {
            this.runLogMethod(getLogMethodName(), message);
        }
        public void Warn(object message, Exception exception)
        {
            this.runLogMethod(getLogMethodName(), message, exception);
        }
        public void WarnFormat(string format, params object[] args)
        {
            this.runLogMethod(getLogMethodName(), format, args);
        }
        public void WarnFormat(string format, object arg0)
        {
            this.runLogMethod(getLogMethodName(), format, arg0);
        }
        public void WarnFormat(string format, object arg0, object arg1)
        {
            this.runLogMethod(getLogMethodName(), format, arg0, arg1);
        }
        public void WarnFormat(string format, object arg0, object arg1, object arg2)
        {
            this.runLogMethod(getLogMethodName(), format, arg0, arg1, arg2);
        }
        public void WarnFormat(IFormatProvider provider, string format, params object[] args)
        {
            this.runLogMethod(getLogMethodName(), provider, format, args);
        }
        #endregion
        #region Error
        public void Error(object message)
        {
            this.runLogMethod(getLogMethodName(), message);
        }
        public void Error(object message, Exception exception)
        {
            this.runLogMethod(getLogMethodName(), message, exception);
        }
        public void ErrorFormat(string format, params object[] args)
        {
            this.runLogMethod(getLogMethodName(), format, args);
        }
        public void ErrorFormat(string format, object arg0)
        {
            this.runLogMethod(getLogMethodName(), format, arg0);
        }
        public void ErrorFormat(string format, object arg0, object arg1)
        {
            this.runLogMethod(getLogMethodName(), format, arg0, arg1);
        }
        public void ErrorFormat(string format, object arg0, object arg1, object arg2)
        {
            this.runLogMethod(getLogMethodName(), format, arg0, arg1, arg2);
        }
        public void ErrorFormat(IFormatProvider provider, string format, params object[] args)
        {
            this.runLogMethod(getLogMethodName(), provider, format, args);
        }
        #endregion
        #region Fatal
        public void Fatal(object message)
        {

            this.runLogMethod(getLogMethodName(), message);
        }
        public void Fatal(object message, Exception exception)
        {
            this.runLogMethod(getLogMethodName(), message, exception);
        }
        public void FatalFormat(string format, params object[] args)
        {
            this.runLogMethod(getLogMethodName(), format, args);
        }
        public void FatalFormat(string format, object arg0)
        {
            this.runLogMethod(getLogMethodName(), format, arg0);
        }
        public void FatalFormat(string format, object arg0, object arg1)
        {
            this.runLogMethod(getLogMethodName(), format, arg0, arg1);
        }
        public void FatalFormat(string format, object arg0, object arg1, object arg2)
        {
            this.runLogMethod(getLogMethodName(), format, arg0, arg1, arg2);
        }
        public void FatalFormat(IFormatProvider provider, string format, params object[] args)
        {
            this.runLogMethod(getLogMethodName(), provider, format, args);
        }
        #endregion

        #endregion
        #endregion
    }
}
