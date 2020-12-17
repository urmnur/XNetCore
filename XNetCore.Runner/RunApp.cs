using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace XNetCore.Runner
{
    public class RunApp
    {
        public static void AppMain(string[] args)
        {
            if (args == null)
            {
                args = new string[0];
            }
            new RunAppHelper().AppMain(args);
        }
    }
    class RunAppHelper
    {
        public void AppMain(string[] args)
        {
            var datetime = DateTime.Now;
            var msg = new StringBuilder();
            msg.Append($"{datetime.ToString("yyyy-MM-dd HH:mm:ss")} >> 应用程序启动").AppendLine(string.Empty);
            if (args != null)
            {
                for (var i = 0; i < args.Length; i++)
                {
                    msg.Append($"                   arg[{i}]={args[i]}").AppendLine(string.Empty);
                }
                Console.WriteLine(msg.ToString());
            }
            try
            {
                runAppMain(args);
            }
            catch (Exception ex)
            {
                msg = new StringBuilder();
                msg.Append($"{datetime.ToString("yyyy-MM-dd HH:mm:ss")} >> 应用程序异常").AppendLine(string.Empty);
                msg.Append(ex.ToString()).AppendLine(string.Empty);
                Console.WriteLine(msg.ToString());
            }
            msg = new StringBuilder();
            msg.Append($"{datetime.ToString("yyyy-MM-dd HH:mm:ss")} >> 应用程序退出").AppendLine(string.Empty);
            Console.WriteLine(msg.ToString());
            Console.ReadLine();
        }

        private void runAppMain(string[] args)
        {
            var admin = IsRunAsAdmin();
            if (!admin)
            {
                runAsAdmin(args);
                return;
            }
            runApp(args);
        }

        #region XNetCore.Runner.RunHelper.Instance.RunApp
        private void runApp(string[] args)
        {
            XNetCore.Runner.RunHelper.Instance.ApplicationArgs = args;
            XNetCore.Runner.RunHelper.Instance.RunApp(string.Empty);
        }
        #endregion

        #region RunAsAdmin
        private void runAsAdmin(string[] args)
        {
            //创建启动对象 
            var startInfo = new System.Diagnostics.ProcessStartInfo();
            //设置运行文件 
            startInfo.FileName = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;
            startInfo.WorkingDirectory = new FileInfo(startInfo.FileName).Directory.FullName;
            //设置启动参数 
            startInfo.Arguments = String.Join(" ", args);
            //设置启动动作,确保以管理员身份运行 
            startInfo.Verb = "runas";
            //如果不是管理员，则启动UAC 
            System.Diagnostics.Process.Start(startInfo);
            //退出 
            System.Windows.Forms.Application.Exit();
        }

        private bool IsRunAsAdmin()
        {
            return true;
            var identity = System.Security.Principal.WindowsIdentity.GetCurrent();
            var principal = new System.Security.Principal.WindowsPrincipal(identity);
            //判断当前登录用户是否为管理员 
            var result = principal.IsInRole(System.Security.Principal.WindowsBuiltInRole.Administrator);
            return result;
        }
        #endregion

    }
}
