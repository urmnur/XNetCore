using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace XNetCore.Runner
{
    class AppKillerHelper
    {
        #region 单例模式
        private static readonly object lockobject = new object();
        private static AppKillerHelper _instance = null;
        public static AppKillerHelper Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (lockobject)
                    {
                        if (_instance == null)
                        {
                            _instance = new AppKillerHelper();
                        }
                    }

                }
                return _instance;
            }
        }
        private AppKillerHelper()
        {
        }
        #endregion


        public void KillCurrentApp()
        {
            try
            {
                Application.ExitThread();
                Application.Exit();
            }
            catch (Exception ex)
            {

            }
            try
            {
                var p = Process.GetCurrentProcess();
                p.Kill();
            }
            catch (Exception ex)
            {

            }
        }
    }
}
