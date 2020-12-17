using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace XNetCore.KApp
{
    class FileData
    {
        public FileInfo File { get; set; }

        public string FileMd5 { get; set; }
    }

    class SyncFileHelper
    {
        #region 单例模式
        private static object lockobject = new object();
        private static SyncFileHelper _instance = null;
        public static SyncFileHelper Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (lockobject)
                    {
                        if (_instance == null)
                        {
                            _instance = new SyncFileHelper();
                        }
                    }

                }
                return _instance;
            }
        }
        private SyncFileHelper()
        {
        }
        #endregion

        private DirectoryInfo[] getdirectories(DirectoryInfo sdir, DirectoryInfo tdir)
        {
            var result = new List<DirectoryInfo>();
            foreach (var d in tdir.GetDirectories())
            {
                if (d.FullName.ToLower()== sdir.FullName.ToLower())
                {
                    continue;
                }
                result.Add(d);
                result.AddRange(getdirectories(sdir, d));
            }
            return result.ToArray();
        }

        private FileData[] getFileData(DirectoryInfo dir, bool filter)
        {
            var result = new List<FileData>();
            foreach (var file in dir.GetFiles())
            {
                var filedata = new FileData();
                filedata.File = file;
                filedata.FileMd5 = Md5Helper.Instance.GetFileMd5(file.FullName);
                result.Add(filedata);
            }
            foreach (var sdir in dir.GetDirectories())
            {
                result.AddRange(getFileData(sdir, filter));
            }
            return result.ToArray();
        }

        private FileData getUpdateFile(FileData appFile, FileData[] updateFiles)
        {
            foreach (var updateFile in updateFiles)
            {
                var b = updateFile.File.Name.ToLower() == appFile.File.Name.ToLower()
                && updateFile.File.FullName.ToLower() != appFile.File.FullName.ToLower()
                && updateFile.FileMd5.ToLower() != appFile.FileMd5.ToLower();
                if (b)
                {
                    return updateFile;
                }
            }
            return null;
        }

        private int getNeedUpdateDirectories(DirectoryInfo dir, FileData[] updateFiles)
        {
            var appFiles = getFileData(dir, true);
            foreach (var appFile in appFiles)
            {
                var updateFile = getUpdateFile(appFile, updateFiles);
                if (updateFile != null)
                {
                    return 1;
                }
            }
            return 0;
        }

        private DirectoryInfo[] getNeedUpdateDirectories(DirectoryInfo[] dirs, FileData[] updateFiles)
        {
            var result = new List<DirectoryInfo>();

            foreach (var dir in dirs)
            {
                var flag = getNeedUpdateDirectories(dir, updateFiles);
                if (flag > 0)
                {
                    result.Add(dir);
                }
            }
            return result.ToArray();
        }
        private int synAppFile(DirectoryInfo dir, FileData[] updateFiles)
        {
            var appFiles = getFileData(dir, true);
            foreach (var appFile in appFiles)
            {
                try
                {
                    var updateFile = getUpdateFile(appFile, updateFiles);
                    if (updateFile != null)
                    {
                        XMQSession.Current.Publish(new XMessage() { MsgData = $"同步文件[{appFile.File.FullName}]" });
                        File.Copy(updateFile.File.FullName, appFile.File.FullName, true);
                    }
                }
                catch (Exception ex)
                {
                    XMQSession.Current.Publish(new XMessage() { MsgData = $"同步文件[{appFile.File.FullName}] 异常\r\n{ex.ToString()}" });
                }
            }
            return 1;
        }
        private int sysApp(DirectoryInfo[] dirs, FileData[] updateFiles)
        {
            var result = 0;
            try
            {
                foreach (var dir in dirs)
                {
                    XMQSession.Current.Publish(new XMessage() { MsgData = $"同步目录[{ dir.FullName }]" });
                    synAppFile(dir, updateFiles);
                    result++;
                }
            }
            catch { }
            return result;
        }

        public int SynUpdateFile(DirectoryInfo sdir, DirectoryInfo tdir)
        {
            var result = 0;
            XMQSession.Current.Publish(new XMessage() { MsgData = $"开始同步文件[{sdir.FullName}]>>[{tdir.FullName}]" });
            try
            {
                result = synUpdateFile(sdir, tdir);
            }
            catch (Exception ex)
            {
                XMQSession.Current.Publish(new XMessage() { MsgData =ex.ToString()});
            }
            XMQSession.Current.Publish(new XMessage() { MsgData = $"完成文件同步[{sdir.FullName}]>>[{tdir.FullName}]" });
            return result;
        }
        private int synUpdateFile(DirectoryInfo sdir, DirectoryInfo tdir)
        {
            if (!sdir.Exists)
            {
                XMQSession.Current.Publish(new XMessage() { MsgData = $"文件夹不存在[{sdir.FullName}]]" });
                return 0;
            }
            if (!tdir.Exists)
            {
                XMQSession.Current.Publish(new XMessage() { MsgData = $"文件夹不存在[{tdir.FullName}]]" });
                return 0;
            }
            var updateFiles = getFileData(sdir, false);
            var dirs = getdirectories(sdir,tdir);
            var updatedirs = getNeedUpdateDirectories(dirs, updateFiles);
            sysApp(updatedirs, updateFiles);
            return 1;
        }
    }
}
