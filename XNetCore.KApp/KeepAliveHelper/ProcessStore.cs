using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace XNetCore.KApp
{
    class ProcessStore
    {
        #region 单例模式
        private static readonly object lockobject = new object();
        private static ProcessStore _instance;
        public static ProcessStore Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (lockobject)
                    {
                        if (_instance == null)
                        {
                            _instance = new ProcessStore();
                        }
                    }
                }
                return _instance;
            }
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        private ProcessStore()
        {
        }
        /// <summary>
        /// 清理数据
        /// </summary>
        public void Reset()
        {
            _instance = null;
        }

        #endregion

        #region GetProcesses
        /// <summary>
        /// 获取所有进程
        /// </summary>
        /// <returns>所有进程</returns>
        private Process[] getWinProcess(string appName)
        {
            var result = new List<Process>();
            try
            {
                var processes = Process.GetProcessesByName(appName);
                foreach (var p in processes)
                {
                    try
                    {
                        result.Add(p);
                    }
                    catch (Exception ex)
                    {
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return result.ToArray();
        }
        /// <summary>
        /// 确定当前进程的完整路径名是否为pathFullname
        /// </summary>
        /// <param name="p">参与比较的进程</param>
        /// <param name="pathFullname">参与比较的路径名称</param>
        /// <returns></returns>
        private bool compareProcess(Process p, string pathFullname)
        {
            try
            {
                if (p.MainModule.FileName.Equals(pathFullname, StringComparison.CurrentCultureIgnoreCase))
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
            }
            return false;
        }


        private string getProcessesName(FileInfo fi)
        {
            return fi.Name.Substring(0, fi.Name.Length - fi.Extension.Length);
        }

        /// <summary>
        /// 通过完整的路径名获取进程
        /// </summary>
        /// <param name="appName"></param>
        /// <returns>进程</returns>
        public Process[] GetProcessesByName(FileInfo fi)
        {
            var result = new List<Process>();
            try
            {
                result.AddRange(getWinProcess(getProcessesName(fi)));
            }
            catch (Exception ex)
            {
            }
            return result.ToArray();
        }
        /// <summary>
        /// 通过完整的路径名获取进程
        /// </summary>
        /// <param name="fi"></param>
        /// <returns>进程</returns>
        public Process[] GetProcesses(FileInfo fi)
        {
            var result = new List<Process>();
            try
            {
                var ps = this.GetProcessesByName(fi);
                foreach (Process p in ps)
                {
                    if (compareProcess(p, fi.FullName))
                    {
                        result.Add(p);
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return result.ToArray();
        }
        #endregion

    }
}
