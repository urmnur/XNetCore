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
    class PrivatePathHelper
    {
        #region 单例模式
        private static readonly object lockobject = new object();
        private static PrivatePathHelper _instance = null;
        public static PrivatePathHelper Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (lockobject)
                    {
                        if (_instance == null)
                        {
                            _instance = new PrivatePathHelper();
                        }
                    }
                }
                return _instance;
            }
        }
        private PrivatePathHelper()
        {
            this.PrivatePaths = getPrivatePaths();
        }
        #endregion

        public DirectoryInfo[] PrivatePaths { get; private set; }

        /// <summary>
        /// 系统路径
        /// </summary>
        private DirectoryInfo getSystemPath()
        {
            var assembly = Assembly.GetEntryAssembly();
            if (assembly == null)
            {
                assembly = this.GetType().Assembly;
            }
            var dir = new FileInfo(new Uri(assembly.Location).LocalPath).Directory;
            if (dir.Name.ToLower() == "bin")
            {
                dir = dir.Parent;
            }
            if (dir.Name.ToLower().EndsWith(".app.bin"))
            {
                dir = dir.Parent;
            }
            return dir;
        }
        /// <summary>
        /// 获取应用程序文件夹
        /// </summary>
        /// <returns></returns>
        private DirectoryInfo[] getPrivatePaths(DirectoryInfo baseDir)
        {
            var result = new List<DirectoryInfo>();
            var fi = new FileInfo(Path.Combine(baseDir.FullName, ".runnerignore"));
            if (fi.Exists)
            {
                return result.ToArray();
            }
            if (checkPuginDir(baseDir))
            {
                return addPrivatePath(baseDir);
            }
            foreach (var dir in baseDir.GetDirectories())
            {
                result.AddRange(getPrivatePaths(dir));
            }
            return result.ToArray();
        }
        /// <summary>
        /// 获取应用程序文件夹
        /// </summary>
        /// <returns></returns>
        private DirectoryInfo[] getPrivatePaths()
        {
            var result = new List<DirectoryInfo>();
            var baseDir = getSystemPath();
            result.Add(baseDir);
            result.AddRange(getPrivatePaths(baseDir));
            return result.ToArray();
        }
        /// <summary>
        /// 应用程序文件夹判定
        /// </summary>
        /// <param name="dir"></param>
        /// <returns></returns>
        private bool checkPuginDir(DirectoryInfo dir)
        {
            var fi = new FileInfo(Path.Combine(dir.FullName, this.GetType().Namespace));
            return fi.Exists;
        }

        /// <summary>
        /// 获取所有应用程序文件夹
        /// </summary>
        /// <param name="baseDir"></param>
        /// <param name="dir"></param>
        /// <returns></returns>
        private DirectoryInfo[] addPrivatePath(DirectoryInfo dir)
        {
            var result = new List<DirectoryInfo>();
            var fi = new FileInfo(Path.Combine(dir.FullName, ".runnerignore"));
            if (fi.Exists)
            {
                return result.ToArray();
            }
            result.Add(dir);
            foreach (var sdir in dir.GetDirectories())
            {
                result.AddRange(addPrivatePath(sdir));
            }
            return result.ToArray();
        }
    }
}