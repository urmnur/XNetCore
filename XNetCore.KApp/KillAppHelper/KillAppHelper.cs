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
    class KillAppHelper
    {
        #region 单例模式
        private static readonly object lockobject = new object();
        private static KillAppHelper _instance;
        public static KillAppHelper Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (lockobject)
                    {
                        if (_instance == null)
                        {
                            _instance = new KillAppHelper();
                        }
                    }

                }
                return _instance;
            }
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        private KillAppHelper()
        {
        }

        #endregion


        public int KillApp(KillAppType type, FileInfo app)
        {
            if (app == null)
            {
                return 0;
            }
            XMQSession.Current.Publish(new XMessage() { MsgData = $"{type.ToString()}  [{app.FullName}]" });
            if (type == KillAppType.AppSuite)
            {
                killprocesses(ProcessStore.Instance.GetProcessesByName(app));
                return 1;
            }
            if (type == KillAppType.AppSelf)
            {
                killprocesses(ProcessStore.Instance.GetProcesses(app));
                return 1;
            }
            if (type == KillAppType.Reboot)
            {
                killprocesses(ProcessStore.Instance.GetProcesses(app));
                return startApp(app);
            }
            killprocesses(ProcessStore.Instance.GetProcesses(app));
            return 1;
        }
        private int startApp(FileInfo app)
        {
            ProcessStartInfo process = new ProcessStartInfo(app.FullName);
            process.UseShellExecute = true;
            process.WindowStyle = ProcessWindowStyle.Minimized;
            process.WorkingDirectory = app.Directory.FullName;
            return System.Diagnostics.Process.Start(process).Id;
        }

        private void killprocesses(Process[] processes)
        {
            if (processes == null)
            {
                return;
            }
            foreach (var p in processes)
            {
                try
                {
                    p.Kill();
                }
                catch (Exception ex)
                {
                }
            }
        }

    }



}
