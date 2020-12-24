using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace XNetCore.Runner
{
    /// <summary>
    /// 程序执行Helper
    /// </summary>
    class RunHelper
    {
        #region 单例模式
        private static readonly object lockobject = new object();
        private static RunHelper _instance = null;
        public static RunHelper Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (lockobject)
                    {
                        if (_instance == null)
                        {
                            _instance = new RunHelper();
                        }
                    }

                }
                return _instance;
            }
        }
        private RunHelper()
        {
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            this.AppIcon = getfavicon();
            sendMsg("RunHelper");
            new PrivatePath().AppendPrivatePaths();
            sendMsg($"AppPath：{Process.GetCurrentProcess().MainModule.FileName}");
            sendMsg("Initialize Runner");
        }
        #endregion

        private void sendMsg(string msg)
        {
            StartLogFileHelper.Instance.StartMsg(msg);
        }

        #region RunApp
        #region RunApp
        public string[] ApplicationArgs { get; set; }
        public bool IsSilent
        {
            get
            {
                var args = this.ApplicationArgs;
                if (args == null)
                {
                    return false;
                }
                foreach (var arg in args)
                {
                    if (string.IsNullOrWhiteSpace(arg))
                    {
                        continue;
                    }
                    if (arg.Trim().ToLower() == "/silent")
                    {
                        return true;
                    }
                }
                return false;
            }
        }
        public void RunApp(string mutexId)
        {
            try
            {
                FormStateHelper.Instance.SetFormState(null);
                FormStateHelper.Instance.SetFormState(SysLogFrm.Instance);
                RepeatFileHelper.Instance.Check();
                runApp(mutexId);
            }
            catch (Exception ex)
            {
                SysLogFrm.Instance.ShowException(ex);
            }
        }
        private void runApp(string mutexId)
        {
            if (this.MainFrm != null)
            {
                return;
            }
            if (string.IsNullOrWhiteSpace(mutexId))
            {
                mutexId = this.GetType().Assembly.CodeBase;
            }

            this.MainFrm = AppRunHelper.Instance.GetAppForm(mutexId);

            if (this.MainFrm != null)
            {
                var msgFliter = new MessageFilter();
                Application.AddMessageFilter(msgFliter);
                msgFliter.Run(this.MainFrm);
                this.MainFrm.FormClosed += MainFrm_FormClosed;
                this.MainFrm.Disposed += MainFrm_Disposed;
                Application.Run(this.MainFrm);
            }
        }

        private void MainFrm_Disposed(object sender, EventArgs e)
        {
        }

        private void MainFrm_FormClosed(object sender, FormClosedEventArgs e)
        {
            AppKillerHelper.Instance.KillCurrentApp();
        }
        #endregion

        #region MainFrm
        public Form MainFrm { get; private set; }
        public IntPtr GetMainFormHandle(DirectoryInfo path)
        {
            return HandleFileHelper.Instance.GetMainFormHandle(path);
        }
        #endregion

        #region Ico
        public event Action<Icon> IconChanged;
        public Icon AppIcon { get { return this.__appIcon; } set { setAppIcon(value); } }
        private Icon __appIcon = null;
        private void setAppIcon(Icon icon)
        {
            if (icon == null)
            {
                return;
            }
            this.__appIcon = icon;
            try { IconChanged?.Invoke(icon); } catch { }
        }

        private Icon getfavicon()
        {
            var appFile = new FileInfo(Process.GetCurrentProcess().MainModule.FileName);
            if (!appFile.Exists)
            {
                return null;
            }
            var faviconFile = new FileInfo(Path.Combine(appFile.Directory.FullName, "favicon.ico"));
            if (faviconFile.Exists)
            {
                try
                {
                    var stream = new FileStream(faviconFile.FullName, FileMode.Open, FileAccess.Read);
                    Bitmap favicon = new Bitmap(stream);
                    stream.Close();
                    stream.Dispose();
                    return Icon.FromHandle(favicon.GetHicon());
                }
                catch { };
            }
            return Icon.ExtractAssociatedIcon(appFile.FullName);
        }

        #endregion
        #endregion
    }

}