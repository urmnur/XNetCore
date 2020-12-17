using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XNetCore.KApp
{
    class KillApp
    {
        public int AppSelf()
        {
            return killApp(KillAppType.AppSelf);
        }
        public int AppSuite()
        {
            return killApp(KillAppType.AppSuite);
        }
        public int AppReboot()
        {
            return killApp(KillAppType.Reboot);
        }

        private int killApp(KillAppType killType)
        {
            var killer = getAppKillPath();
            if (killer == null)
            {
                return 0;
            }
            var app = new FileInfo(Process.GetCurrentProcess().MainModule.FileName);
            ProcessStartInfo process = new ProcessStartInfo(killer.FullName);
            process.UseShellExecute = true;
            process.WindowStyle = ProcessWindowStyle.Minimized;
            process.WorkingDirectory = app.Directory.FullName;
            process.Arguments = $"\"KillType = {((int)killType).ToString()}\" \"AppPath = {app.FullName}\"";
            System.Diagnostics.Process.Start(process);
            return 1;
        }

        private FileInfo getAppKillPath()
        {
            var path = new FileInfo(new Uri(this.GetType().Assembly.CodeBase).LocalPath);
            var file = getAppPath(".KApp.exe".ToLower(), path.Directory);
            if (file == null)
            {
                return null;
            }
            return file;
        }
        private FileInfo getAppPath(string endwith, DirectoryInfo dir)
        {
            var fs = dir.GetFiles();
            foreach (var f in fs)
            {
                if (f.FullName.ToLower().EndsWith(endwith))
                {
                    return f;
                }
            }
            var cdirs = dir.GetDirectories();
            foreach(var cdir in cdirs)
            {
                var result = getAppPath(endwith, cdir);
                if (result != null)
                {
                    return result;
                }
            }
            return null;
        }
    }
    enum KillAppType
    {
        AppSelf = 0,
        AppSuite = 1,
        Reboot = 2,
    }
}
