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
    public static class XLog
    {
        public static int Log(Log log)
        {
            return XLogH.Instance.Write(log);
        }
        public static int Message(string msg)
        {
            return XLogH.Instance.Write(new Log() { Message = msg });
        }
        public static int LogData(object data)
        {
            return XLogH.Instance.Write(new Log() { LogData = data });
        }
        public static int Exception(Exception ex)
        {
            return XLogH.Instance.Write(new Log() { Exception = ex });
        }
    }


    public class Log
    {
        public Log()
        {
            this.LogTime = DateTime.Now;
            this.Level = LogLevel.Trace;
        }
        public int StackFrameIndex { get; set; }        
        public DateTime LogTime { get; private set; }
        public LogLevel Level { get; set; }
        public string Method { get; set; }
        public string Message { get; set; }
        public Exception Exception { get; set; }
        public object LogData { get; set; }


        public override string ToString()
        {
            var log = this;
            var result = new StringBuilder();
            result.Append($"{log.LogTime.ToString("yyyy-MM-dd HH:mm:ss.fff")}");
            result.Append($"[{log.Method}]>>");
            var firstMsg = true;
            if (!string.IsNullOrWhiteSpace(log.Message))
            {
                if (!firstMsg)
                {
                    result.AppendLine(string.Empty);
                }
                result.Append($"Message>>>{log.Message}");
                firstMsg = false;
            }
            if (log.LogData != null)
            {
                if (!firstMsg)
                {
                    result.AppendLine(string.Empty);
                }
                result.Append($"Data>>>{log.LogData.ToIndentedJson()}");
                firstMsg = false;
            }
            if (log.Exception != null)
            {
                if (!firstMsg)
                {
                    result.AppendLine(string.Empty);
                }
                result.Append($"Exception>>>{log.Exception.ToString()}");
                firstMsg = false;
            }
            return result.ToString();
        }
    }
    public enum LogLevel
    {
        Trace = 1,
        Debug = 2,
        Info = 3,
        Warn = 4,
        Error = 5,
        Fatal = 6,
    }
}
