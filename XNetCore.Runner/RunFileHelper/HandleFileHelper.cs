using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace XNetCore.Runner
{
    class HandleFileHelper
    {
        #region 单例模式
        private static readonly object lockobject = new object();
        private static HandleFileHelper _instance = null;
        public static HandleFileHelper Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (lockobject)
                    {
                        if (_instance == null)
                        {
                            _instance = new HandleFileHelper();
                        }
                    }

                }
                return _instance;
            }
        }
        private HandleFileHelper()
        {
        }
        #endregion

        private const string CONST_MAIN_FORM_HANDLE = "MainFrm=";
        private const string CONST_STSRT_FORM_HANDLE = "StartFrm=";

        public void Save(Form frm)
        {
            var file = getRunningFile();
            if (file.Exists)
            {
                file.Delete();
            }
            appendText($"ProcessId={Process.GetCurrentProcess().Id}");
            appendText($"{CONST_MAIN_FORM_HANDLE}{frm.Handle.ToInt64()}");
            appendText($"{CONST_STSRT_FORM_HANDLE}{SysLogFrm.Instance.Handle.ToInt64()}");
            StartLogFileHelper.Instance.StartMsg($"ProcessId[{Process.GetCurrentProcess().Id}].MainFrm[{frm.Handle.ToInt64()}]");
        }


        private void appendText(string msg)
        {
            try
            {
                msg = $"{msg}\r\n";
                System.Console.WriteLine(msg);
                System.Diagnostics.Trace.WriteLine(msg);
                var file = getRunningFile();
                File.AppendAllText(file.FullName, msg);
            }
            catch { }
        }

        private string getFileName()
        {
            var fileName = this.GetType().Namespace;
            fileName = fileName.Substring(0, fileName.IndexOf(".") + 1) + "handle";
            return fileName;
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
            file = new FileInfo(Path.Combine(dir.FullName, getFileName()));
            return file;
        }
        public void ShowMainForm(DirectoryInfo path)
        {
            try
            {
                showMainForm(path);
            }
            catch (Exception ex)
            { }
        }
        private IntPtr getStartFormHandle(DirectoryInfo path)
        {
            var file = new FileInfo(Path.Combine(path.FullName, getFileName()));
            if (file == null || !file.Exists)
            {
                return IntPtr.Zero;
            }
            var ss = File.ReadAllLines(file.FullName);
            if (ss == null || ss.Length == 0)
            {
                return IntPtr.Zero;
            }
            var handle = IntPtr.Zero;
            foreach (var s in ss)
            {
                var str = s.ToLower().Trim();
                if (!str.StartsWith(CONST_STSRT_FORM_HANDLE.ToLower()))
                {
                    continue;
                }
                str = str.Substring(CONST_STSRT_FORM_HANDLE.Length);
                if (int.TryParse(str, out int h))
                {
                    handle = new IntPtr(h);
                }
                break;
            }
            return handle;
        }

        public IntPtr GetMainFormHandle(DirectoryInfo path)
        {
            var file = new FileInfo(Path.Combine(path.FullName, getFileName()));
            if (file == null || !file.Exists)
            {
                return IntPtr.Zero;
            }
            var ss = File.ReadAllLines(file.FullName);
            if (ss == null || ss.Length == 0)
            {
                return IntPtr.Zero;
            }
            var handle = IntPtr.Zero;
            foreach (var s in ss)
            {
                var str = s.ToLower().Trim();
                if (!str.StartsWith(CONST_MAIN_FORM_HANDLE.ToLower()))
                {
                    continue;
                }
                str = str.Substring(CONST_MAIN_FORM_HANDLE.Length);
                if (int.TryParse(str, out int h))
                {
                    handle = new IntPtr(h);
                }
                break;
            }
            return handle;
        }

        
        private void showMainForm(DirectoryInfo path)
        {
            var handle = getStartFormHandle(path);
            if (handle == IntPtr.Zero)
            {
                return;
            }
            SysLogFrm.Instance.ShowMainForm(handle);
        }
    }
}
