using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XNetCore.Runner
{
    class AutoRunHelper
    {
        #region 单例模式
        private static readonly object lockobject = new object();
        private static AutoRunHelper _instance = null;
        public static AutoRunHelper Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (lockobject)
                    {
                        if (_instance == null)
                        {
                            _instance = new AutoRunHelper();
                        }
                    }

                }
                return _instance;
            }
        }
        private AutoRunHelper()
        {
        }
        #endregion

        /// <summary>
        /// 将本程序设为开启自启
        /// </summary>
        /// <param name="onOff">自启开关</param>
        /// <returns></returns>
        public bool SetAutoStart(bool onOff)
        {
            var appName = Process.GetCurrentProcess().MainModule.ModuleName;
            var appPath = Process.GetCurrentProcess().MainModule.FileName;
            var result = SetAutoStart(onOff, appName, appPath);
            return result;
        }

        /// <summary>
        /// 将应用程序设为或不设为开机启动
        /// </summary>
        /// <param name="onOff">自启开关</param>
        /// <param name="appName">应用程序名</param>
        /// <param name="appPath">应用程序完全路径</param>
        public bool SetAutoStart(bool onOff, string appName, string appPath)
        {
            var result = true;
            StartLogFileHelper.Instance.StartMsg("SetAutoStart:" + onOff.ToString());
            //如果从没有设为开机启动设置到要设为开机启动
            if (!IsExistKey(appName) && onOff)
            {
                result = SelfRunning(onOff, appName, appPath);
            }
            //如果从设为开机启动设置到不要设为开机启动
            else if (IsExistKey(appName) && !onOff)
            {
                result = SelfRunning(onOff, appName, appPath);
            }
            return result;
        }
        public bool GetAutoStart()
        {
            var appName = Process.GetCurrentProcess().MainModule.ModuleName;
            var result = GetAutoStart(appName);
            return result;
        }
        public bool GetAutoStart(string appName)
        {
            if (IsExistKey(appName))
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        /// <summary>
        /// 判断注册键值对是否存在，即是否处于开机启动状态
        /// </summary>
        /// <param name="keyName">键值名</param>
        /// <returns></returns>
        private bool IsExistKey(string keyName)
        {
            try
            {
                var result = false;
                var local = Registry.LocalMachine;
                var runs = local.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);
                if (runs == null)
                {
                    var key2 = local.CreateSubKey("SOFTWARE");
                    var key3 = key2.CreateSubKey("Microsoft");
                    var key4 = key3.CreateSubKey("Windows");
                    var key5 = key4.CreateSubKey("CurrentVersion");
                    var key6 = key5.CreateSubKey("Run");
                    runs = key6;
                }
                var runsName = runs.GetValueNames();
                foreach (string strName in runsName)
                {
                    if (strName.ToUpper() == keyName.ToUpper())
                    {
                        result = true;
                        return result;
                    }
                }
                return result;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 写入或删除注册表键值对,即设为开机启动或开机不启动
        /// </summary>
        /// <param name="isStart">是否开机启动</param>
        /// <param name="exeName">应用程序名</param>
        /// <param name="path">应用程序路径带程序名</param>
        /// <returns></returns>
        private bool SelfRunning(bool isStart, string exeName, string path)
        {
            try
            {
                var local = Registry.LocalMachine;
                var key = local.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);
                if (key == null)
                {
                    local.CreateSubKey("SOFTWARE//Microsoft//Windows//CurrentVersion//Run");
                }
                //若开机自启动则添加键值对
                if (isStart)
                {
                    key.SetValue(exeName, "\"" + path + "\"");
                    key.Close();
                }
                else//否则删除键值对
                {
                    var keyNames = key.GetValueNames();
                    foreach (string keyName in keyNames)
                    {
                        if (keyName.ToUpper() == exeName.ToUpper())
                        {
                            key.DeleteValue(exeName);
                            key.Close();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                StartLogFileHelper.Instance.StartMsg("SetAutoStart:" + ex.ToString());
                return false;
            }

            return true;
        }
    }
}
