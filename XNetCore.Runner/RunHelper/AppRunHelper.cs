using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace XNetCore.Runner
{
    class AppRunHelper
    {
        #region 单例模式
        private static readonly object lockobject = new object();
        private static AppRunHelper _instance = null;
        public static AppRunHelper Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (lockobject)
                    {
                        if (_instance == null)
                        {
                            _instance = new AppRunHelper();
                        }
                    }
                }
                return _instance;
            }
        }
        private AppRunHelper()
        {
        }
        #endregion

        private void sendMsg(string msg)
        {
            StartLogFileHelper.Instance.StartMsg(msg);
        }
        public Form GetAppForm(string mutexId)
        {
            try
            {
                if (isMutexId(mutexId))
                {
                    AppKillerHelper.Instance.KillCurrentApp();
                    return null;
                }
                appException();
                return getAppForm();
            }
            catch (Exception ex)
            {
            }
            return null;
        }

        #region MutexId
        private bool isMutexId(string mutexId)
        {
            sendMsg("初始化系统运行界面……");
            if (!string.IsNullOrWhiteSpace(mutexId))
            {
                sendMsg("进程互斥变量=" + mutexId);
                if (new AppMutex().IsExist(mutexId))
                {
                    sendMsg($"系统已运行，不允许重复运行！");
                    var path = getAppPath();
                    HandleFileHelper.Instance.ShowMainForm(path);
                    return true;
                    if (!RunHelper.Instance.IsSilent)
                    {
                        MessageBox.Show($"{path.FullName}\r\n\r\n系统不允许重复执行！", $"系统已运行");
                    }
                    return true;
                }
            }
            return false;
        }


        /// <summary>
        /// 系统路径
        /// </summary>
        private DirectoryInfo getAppPath()
        {
            var assembly = Assembly.GetEntryAssembly();
            if (assembly == null)
            {
                assembly = this.GetType().Assembly;
            }
            var app = new FileInfo(new Uri(assembly.CodeBase).LocalPath);
            return app.Directory;
        }

        #endregion

        #region getAppForm
        private Form getAppForm()
        {
            var frm = AppFormHelper.Instance.GetAppForm();
            HandleFileHelper.Instance.Save(frm);
            FormStateHelper.Instance.SetFormState(frm);
            return frm;
        }
        #endregion

        #region ThreadException
        private void appException()
        {
            try
            {
                Application.ThreadException += Application_ThreadException;
            }
            catch { }

            try
            {
                AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            }
            catch { }
        }



        private void showExceptionFrm(Exception ex)
        {
            SysLogFrm.Instance.ShowException(ex);
        }



        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var ex = e.ExceptionObject as Exception;
            sendMsg($"系统未处理异常！>>\r\n" + ex.ToString());
            showExceptionFrm(ex);

        }

        private void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            var ex = e.Exception as Exception;
            sendMsg($"进程未处理异常！>>\r\n" + ex.ToString());
            showExceptionFrm(ex);
        }
        #endregion
    }
}
