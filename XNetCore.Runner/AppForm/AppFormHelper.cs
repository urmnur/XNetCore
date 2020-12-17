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
    class AppFormHelper
    {
        #region 单例模式
        private static readonly object lockobject = new object();
        private static AppFormHelper _instance = null;
        public static AppFormHelper Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (lockobject)
                    {
                        if (_instance == null)
                        {
                            _instance = new AppFormHelper();
                        }
                    }
                }
                return _instance;
            }
        }
        private AppFormHelper()
        {
        }
        #endregion



        public Form GetAppForm()
        {
            try
            {
                return getAppForm();
            }
            catch (Exception ex)
            {
            }
            return null;
        }

      

        private Form getAppForm()
        {
            Form result = null;
            try
            {
                result = RunnerFrmHelper.Instance.GetAppForm();
                if (result != null)
                {
                    return result;
                }
            }
            catch (Exception ex)
            {
                sendMsg(ex.ToString());
            }
            try
            {
                result = TestFrmHelper.Instance.GetAppForm();
                if (result != null)
                {
                    return result;
                }
            }
            catch (Exception ex)
            {
                sendMsg(ex.ToString());
            }
            try
            {
                result = LocalFrmHelper.Instance.GetAppForm();
                if (result != null)
                {
                    return result;
                }
            }
            catch (Exception ex)
            {
                sendMsg(ex.ToString());
            }
            try
            {
                result = LocalSubFrmHelper.Instance.GetAppForm();
                if (result != null)
                {
                    return result;
                }
            }
            catch (Exception ex)
            {
                sendMsg(ex.ToString());
            }
            return SysLogFrm.Instance;
        }


        private void sendMsg(string msg)
        {
            StartLogFileHelper.Instance.StartMsg(msg);
        }
    }
}
