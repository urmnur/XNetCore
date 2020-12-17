using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XNetCore.Runner
{
    class RepeatFileHelper
    {
        #region 单例模式
        private static readonly object lockobject = new object();
        private static RepeatFileHelper _instance = null;
        public static RepeatFileHelper Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (lockobject)
                    {
                        if (_instance == null)
                        {
                            _instance = new RepeatFileHelper();
                        }
                    }
                }
                return _instance;
            }
        }
        private RepeatFileHelper()
        {
        }
        #endregion

        public void Check()
        {
            repeatFileException(getFiles(PrivatePathHelper.Instance.PrivatePaths));
        }

        class ManComparerNew : IEqualityComparer<FileInfo>
        {
            public bool Equals(FileInfo x, FileInfo y)
            {
                if(x.Directory.Name.ToLower()== "locales")
                {
                    return false;
                }
                return x.FullName.Equals(y.FullName, StringComparison.OrdinalIgnoreCase);
            }

            public int GetHashCode(FileInfo obj)
            {
                if (obj == null)
                    return 0;
                else
                    return obj.FullName.GetHashCode();
            }
        }

        private void repeatFileException(FileInfo[] files)
        {
            files = files.Distinct(new ManComparerNew()).ToArray();
            var gfiles = files.GroupBy((e) => { return e.Name.ToLower(); });
            var errsb = new StringBuilder();
            foreach (var gfile in gfiles)
            {
                if (gfile.Count() > 1)
                {
                    errsb.Append(gfile.FirstOrDefault().Name);
                    errsb.Append(">>>>>>>>>>>>>>>>>").AppendLine(string.Empty);
                    foreach (var f in gfile)
                    {
                        errsb.Append(f.FullName).AppendLine(string.Empty);
                    }
                    errsb.Append(gfile.FirstOrDefault().Name);
                    errsb.Append("<<<<<<<<<<<<<<<<").AppendLine(string.Empty);
                }
            }
            if (errsb.Length > 0)
            {
                throw new Exception("重复类库文件\r\n\r\n" + errsb.ToString());
            }
        }
        private FileInfo[] getFiles(DirectoryInfo[] dirs)
        {
            var result = new List<FileInfo>();
            foreach (var dir in dirs)
            {
                result.AddRange(getFiles(dir));
            }
            return result.ToArray();

        }
        private FileInfo[] getFiles(DirectoryInfo dir)
        {
            var result = new List<FileInfo>();
            if (!dir.Exists)
            {
                return result.ToArray();
            }
            var pdir = dir;
            while (true)
            {
                if (isPrivateDir(pdir) == 0)
                {
                    return result.ToArray();
                }
                pdir = pdir.Parent;
                if (pdir==null)
                {
                    break;
                }
            }

            foreach (var file in dir.GetFiles())
            {
                if (isPrivateFile(file) == 1)
                {
                    result.Add(file);
                }
            }
            return result.ToArray();
        }

        private int isPrivateDir(DirectoryInfo dir)
        {
            var result = 1;
            if (dir.Name.ToLower().StartsWith("microsoft"))
            {
                return 0;
            }
            if (dir.Name.ToLower().EndsWith("x64"))
            {
                return 0;
            }
            if (dir.Name.ToLower().EndsWith("x86"))
            {
                return 0;
            }
            return result;
        }
        private int isPrivateFile(FileInfo fi)
        {
            var result = 0;
            if (fi.Name.ToLower().EndsWith(".dll"))
            {
                return 1;
            }
            if (fi.Name.ToLower().EndsWith(".exe"))
            {
                return 1;
            }
            return result;
        }


    }
}
