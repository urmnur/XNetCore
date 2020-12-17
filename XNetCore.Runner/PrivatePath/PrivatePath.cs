using Microsoft.Extensions.DependencyModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace XNetCore.Runner
{
    /// <summary>
    /// 程序应用程序文件夹
    /// </summary>
    class PrivatePath
    {
        private void writeMsg(string msg)
        {
            msg = $"PrivatePath:{msg}";
            StartLogFileHelper.Instance.StartMsg(msg);
        }
        /// <summary>
        /// 设置应用程序文件夹
        /// </summary>
        /// <returns></returns>
        public void SetPrivatePaths()
        {
            var paths = PrivatePathHelper.Instance.PrivatePaths;
            foreach (var dir in paths)
            {
                writeMsg(dir.FullName);
                setPrivatePath(dir);
            }
        }

        /// <summary>
        /// 设置应用程序文件夹
        /// </summary>
        /// <param name="dir"></param>
        private void setPrivatePath(DirectoryInfo dir)
        {
            try
            {
                foreach (var f in dir.GetFiles())
                {
                    try
                    {
                        var assm = Assembly.LoadFrom(f.FullName);
                        if (assm != null)
                        {
                            DependencyContext.Load(assm);
                        }
                    }
                    catch
                    {

                    }
                }
            }
            catch
            {

            }
        }

        /// <summary>
        /// 设置应用程序文件夹
        /// </summary>
        /// <param name="dir"></param>
        private void setPrivatePath0(DirectoryInfo dir)
        {

            AppDomain.CurrentDomain.AppendPrivatePath(dir.FullName);
        }
        /// <summary>
        /// 设置应用程序文件夹  反射
        /// 暂不使用
        /// </summary>
        /// <param name="path"></param>
        private void setPrivatePath1(DirectoryInfo dir)
        {
            var path = dir.FullName;
            AppDomain.CurrentDomain.SetData("PRIVATE_BINPATH", path);
            AppDomain.CurrentDomain.SetData("BINPATH_PROBE_ONLY", path);
            var m = typeof(AppDomainSetup).GetMethod("UpdateContextProperty", BindingFlags.NonPublic | BindingFlags.Static);
            var funsion = typeof(AppDomain).GetMethod("GetFusionContext", BindingFlags.NonPublic | BindingFlags.Instance);
            m.Invoke(null, new object[] { funsion.Invoke(AppDomain.CurrentDomain, null), "PRIVATE_BINPATH", path });
        }
    }
}