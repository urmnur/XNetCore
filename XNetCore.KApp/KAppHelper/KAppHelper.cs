using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace XNetCore.KApp
{
    class KAppHelper
    {
        #region 单例模式
        private static readonly object lockobject = new object();
        private static KAppHelper _instance;
        public static KAppHelper Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (lockobject)
                    {
                        if (_instance == null)
                        {
                            _instance = new KAppHelper();
                        }
                    }

                }
                return _instance;
            }
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        private KAppHelper()
        {

        }
        #endregion


        private string[] arg = null;
        public void SetAppArguments(string[] arg)
        {
            this.arg = arg;
        }
        private string getArgsValue(string key)
        {
            var args = this.arg;
            if (args == null || args.Length == 0)
            {
                return string.Empty;
            }
            foreach (var arg in args)
            {
                if (string.IsNullOrWhiteSpace(arg))
                {
                    continue;
                }
                var idx = arg.IndexOf("=");
                if (idx < 0)
                {
                    continue;
                }
                var argKey = arg.Substring(0, idx).Trim();
                var argValue = arg.Substring(idx + 1);
                if (argKey.ToLower() == key.ToLower())
                {
                    if (string.IsNullOrWhiteSpace(argValue))
                    {
                        return string.Empty;
                    }
                    return argValue.Trim();
                }
            }
            return string.Empty;
        }

        public bool AutoClose { get { return this.arg != null; } }



        public FileInfo AppPath { get { return getAppPath(); } }

        private FileInfo __AppPath = null;

        private FileInfo getAppPath()
        {
            if (this.__AppPath != null)
            {
                return this.__AppPath;
            }
            var result = this.__AppPath;
            var appPath = getArgsValue("AppPath");
            if (!string.IsNullOrWhiteSpace(appPath))
            {
                try
                {
                    result = new FileInfo(appPath);
                    if (result != null && result.Exists
                         && !result.FullName.Equals(LocalPath.ProcessPath.FullName, StringComparison.OrdinalIgnoreCase))
                    {
                        return this.__AppPath = result;
                    }
                }
                catch { }
            }
            result = getAppPath(LocalPath.ProcessPath.Directory, ".App.exe".ToLower());
            if (result != null && result.Exists
                && !result.FullName.Equals(LocalPath.ProcessPath.FullName, StringComparison.OrdinalIgnoreCase))
            {
                return this.__AppPath = result;
            }
            result = getAppPath(LocalPath.ProcessPath.Directory, ".exe".ToLower());
            if (result != null && result.Exists
                && !result.FullName.Equals(LocalPath.ProcessPath.FullName, StringComparison.OrdinalIgnoreCase))
            {
                return this.__AppPath = result;
            }
            return this.__AppPath;
        }

        private FileInfo getAppPath(DirectoryInfo dir, string endwith)
        {
            var fs = dir.GetFiles();
            foreach (var f in fs)
            {
                if (f.FullName.ToLower().EndsWith(endwith))
                {
                    return f;
                }
            }
            var pdir = dir.Parent;
            if (pdir == null)
            {
                return null;
            }
            return getAppPath(pdir, endwith);
        }


        public KillAppType KillAppType { get { return getKillAppType(); } }

        private KillAppType getKillAppType()
        {
            var result = getArgsValue("KillType");
            if (!string.IsNullOrWhiteSpace(result))
            {
                return KillAppType.AppSelf;
            }
            if (!int.TryParse(result, out int type))
            {
                return KillAppType.AppSelf;
            }
            try { return (KillAppType)type; } catch { }
            return KillAppType.AppSelf;
        }
    }

}
