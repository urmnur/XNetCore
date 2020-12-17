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
    class KeepAliveHelper
    {
        #region 单例模式
        private static readonly object lockobject = new object();
        private static KeepAliveHelper _instance;
        public static KeepAliveHelper Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (lockobject)
                    {
                        if (_instance == null)
                        {
                            _instance = new KeepAliveHelper();
                        }
                    }

                }
                return _instance;
            }
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        private KeepAliveHelper()
        {
        }
        #endregion

        public int KeepAlive()
        {
            if (KAppHelper.Instance.AppPath==null)
            {
                return 0;
            }
            return this.keepAlive(KAppHelper.Instance.AppPath);
        }
        private int keepAlive(FileInfo app)
        {
            var result = getAppProcessId(app);
            if (result > 0)
            {
                return result;
            }
            var p = runApp(app);
            return p.Id;
        }
        private int getAppProcessId(FileInfo app)
        {
            var ps = ProcessStore.Instance.GetProcesses(app);
            if (ps == null || ps.Length == 0)
            {
                return 0;
            }
            return ps[0].Id;
        }

        private Process runApp(FileInfo app)
        {
            XMQSession.Current.Publish(new XMessage() { MsgData = $"RunApp [{app.FullName}]" });
            //创建启动对象 
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            //设置运行文件 
            startInfo.FileName = app.FullName;
            //设置启动动作,确保以管理员身份运行 
            startInfo.Verb = "runas";
            //如果不是管理员，则启动UAC 
            return System.Diagnostics.Process.Start(startInfo);
        }
    }

}
