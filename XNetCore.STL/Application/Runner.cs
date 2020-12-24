using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace XNetCore.STL
{
    /// <summary>
    /// 程序应用程序文件夹
    /// </summary>
    public class Runner
    {
        #region 单例模式
        private static readonly object lockobject = new object();
        private static Runner _instance = null;
        public static Runner Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (lockobject)
                    {
                        if (_instance == null)
                        {
                            _instance = new Runner();
                        }
                    }
                }
                return _instance;
            }
        }
        private Runner()
        {
            this.PrivatePaths = getProperty<DirectoryInfo[]>("PrivatePaths");

            if (this.PrivatePaths == null)
            {
                this.PrivatePaths = new DirectoryInfo[0];
            }
            this.PrivateFiles = getPrivateFiles(this.PrivatePaths);
        }
        #endregion

        public void AppendPrivatePaths()
        {
            var type = "XNetCore.Runner.PrivatePath,XNetCore.Runner".ToType();
            if(type==null)
            {
                return;
            }
            var instance = type.Instance();
            if (instance == null)
            {
                return;
            }
            var method = type.GetMethod("AppendPrivatePaths");
            if (method == null)
            {
                return;
            }
            method.Invoke(instance, null);
        }

        public DirectoryInfo[] PrivatePaths { get; private set; }


        public FileInfo[] PrivateFiles { get; private set; }

        private FileInfo[] getPrivateFiles(DirectoryInfo[] dirs)
        {
            var result = new List<FileInfo>();
            foreach (var dir in dirs)
            {
                var fs = dir.GetFiles();
                foreach (var f in fs)
                {
                    if (f.Extension.ToLower() == ".dll")
                    {
                        result.Add(f);
                    }
                }
            }
            return result.ToArray();
        }

        private object getRunnerInstance()
        {
            var type = "XNetCore.Runner.PrivatePathHelper,XNetCore.Runner".ToType();
            if (type == null)
            {
                return null;
            }
            var p = type.Property("Instance");
            if (p == null)
            {
                return null;
            }
            var instance = p.GetValue(null);
            if (instance == null)
            {
                return null;
            }
            return instance;
        }

        private T getProperty<T>(string pname)
        {
            var instance = getRunnerInstance();
            if (instance == null)
            {
                return default(T);
            }
            var type = instance.GetType();
            var p = type.Property(pname);
            var result = p.GetValue(instance);
            return (T)result;
        }

        private ConcurrentDictionary<FileInfo, Type[]> fileInfoTypes = new ConcurrentDictionary<FileInfo, Type[]>();
        public Type[] GetFileTypes(FileInfo fi)
        {
            if (this.fileInfoTypes.ContainsKey(fi))
            {
                this.fileInfoTypes.TryGetValue(fi, out Type[] value);
                return value;
            }
            var result = mGetFileTypes(fi);
            this.fileInfoTypes.TryAdd(fi, result);
            return result;
        }
        private Type[] mGetFileTypes(FileInfo fi)
        {
            var result = new List<Type>();
            try
            {
                var ass = Assembly.LoadFile(fi.FullName);
                var ts = ass.GetTypes();
                foreach (var t in ts)
                {
                    var type = t.AssemblyQualifiedName.ToType();
                    if (type != null)
                    {
                        result.Add(type);
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return result.ToArray();
        }
    }
}