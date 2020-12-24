using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XNetCore.STL
{
    class AboutHelper
    {
        #region 单例模式
        private static readonly object lockobject = new object();
        private static AboutHelper _instance = null;
        public static AboutHelper Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (lockobject)
                    {
                        if (_instance == null)
                        {
                            _instance = new AboutHelper();
                        }
                    }

                }
                return _instance;
            }
        }
        private AboutHelper()
        {
            this.___system_about = mGetSystemAbout();
        }
        #endregion

        private SystemAbout ___system_about = null;
        public SystemAbout GetSystemAbout()
        {
            if (___system_about == null)
            {
                ___system_about = mGetSystemAbout();
            }
            return ___system_about;
        }
        public SystemAbout mGetSystemAbout()
        {
            var versions = new List<FileVersion>();
            var fs = Runner.Instance.PrivateFiles;
            var dir = LocalPath.ProcessFile;
            var maxTime = DateTime.MinValue;
            for (var i = 0; i < fs.Length; i++)
            {
                try
                {
                    var f = fs[i];
                    var name = f.FullName.Substring(dir.FullName.Length);
                    var dllTime = f.LastWriteTime;
                    versions.Add(new FileVersion(name, dllTime));
                    if (dllTime > maxTime)
                    {
                        maxTime = dllTime;
                    }

                }
                catch (Exception ex)
                {

                }
            }
            return new SystemAbout(maxTime, versions.ToArray());
        }


    }


    class FileVersion
    {
        public FileVersion(string filePath, DateTime lastWriteTime)
        {
            this.FilePath = filePath;
            this.LastWriteTime = lastWriteTime;
        }
        public string FilePath { get; private set; }
        public DateTime LastWriteTime { get; private set; }
    }
    class SystemAbout
    {
        public SystemAbout(DateTime lastWriteTime, FileVersion[] versions)
        {
            this.LastWriteTime = lastWriteTime;
            this.FileVersions = versions;
        }
        public DateTime LastWriteTime { get; private set; }
        public FileVersion[] FileVersions { get; private set; }
    }

}
