using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace XNetCore.STL
{
    /// <summary>
    /// 路径
    /// </summary>
    public class LocalPath
    {
        /// <summary>
        /// 当前dll所在路径
        /// </summary>
        public static FileInfo CurrentFile
        {
            get
            {
                var st = new StackTrace();
                var sfs = st.GetFrames();
                var method = sfs[1].GetMethod();
                var type = method.ReflectedType;
                var fi = new FileInfo(new Uri(type.Assembly.CodeBase).LocalPath);
                return fi;
            }
        }


        /// <summary>
        /// 当前dll所在路径
        /// </summary>
        public static FileInfo TypePath(Type type)
        {
            var fi = new FileInfo(type.Assembly.Location);
            return fi;
        }


        private static FileInfo __process_file;
        /// <summary>
        /// 系统路径
        /// </summary>
        public static FileInfo ProcessFile
        {
            get
            {
                if (__process_file != null)
                {
                    return __process_file;
                }
                var fileName = Process.GetCurrentProcess().MainModule.FileName;
                return __process_file = new FileInfo(fileName);
            }
        }

        /// <summary>
        /// 系统路径
        /// </summary>
        public static string FirstName
        {
            get
            {
                var firstName = typeof(LocalPath).Namespace;
                firstName = firstName.Substring(0, firstName.IndexOf("."));
                return firstName;
            }
        }

        private static DirectoryInfo __current_temp_path;
        /// <summary>
        /// 系统路径
        /// </summary>
        public static DirectoryInfo CurrentTempPath
        {
            get
            {
                if (__current_temp_path != null)
                {
                    return __current_temp_path;
                }
                __current_temp_path = getCurrentTempPath();
                return __current_temp_path;
            }
        }

        private static DirectoryInfo getCurrentTempPath()
        {
            var appName = FirstName + "." + "TempFile";
            var dir = Path.Combine(ProcessFile.Directory.FullName, appName);
            var result = new DirectoryInfo(dir);
            if (!result.Exists)
            {
                result.Create();
            }
            return result;
        }
    }
}
