using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XNetCore.Runner
{
    class StartLogFileHelper
    {
        #region 单例模式
        private static readonly object lockobject = new object();
        private static StartLogFileHelper _instance = null;
        public static StartLogFileHelper Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (lockobject)
                    {
                        if (_instance == null)
                        {
                            _instance = new StartLogFileHelper();
                        }
                    }

                }
                return _instance;
            }
        }
        private StartLogFileHelper()
        {
            var runningFile = getRunningFile();
            if (runningFile.Exists)
            {
                runningFile.Delete();
            }
        }

        #endregion

        public void StartMsg(string msg)
        {
            try
            {
                appendText(msg);
                SysLogFrm.Instance.ShowMsg(msg);
            }
            catch { }
        }



        private void appendText(string msg)
        {
            try
            {
                msg = $"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")} >> {msg}\r\n";
                System.Console.WriteLine(msg);
                System.Diagnostics.Trace.WriteLine(msg);
                var file = this.getRunningFile();
                File.AppendAllText(file.FullName, msg);
            }
            catch { }
        }
        private FileInfo getRunningFile()
        {
            var appPath = new FileInfo(new Uri(this.GetType().Assembly.CodeBase).LocalPath);
            var file = new FileInfo(new Uri(this.GetType().Assembly.CodeBase).LocalPath);
            var dir = file.Directory;
            if (dir.Name.ToLower() == "bin")
            {
                dir = dir.Parent;
            }
            var fileName = this.GetType().Namespace;
            fileName = fileName.Substring(0, fileName.IndexOf(".") + 1) + "Running";
            file = new FileInfo(Path.Combine(dir.FullName, fileName));
            return file;
        }
    }
}
