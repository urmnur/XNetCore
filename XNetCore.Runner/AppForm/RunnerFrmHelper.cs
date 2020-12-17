using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace XNetCore.Runner
{
    class RunnerFrmHelper
    {
        #region 单例模式
        private static readonly object lockobject = new object();
        private static RunnerFrmHelper _instance = null;
        public static RunnerFrmHelper Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (lockobject)
                    {
                        if (_instance == null)
                        {
                            _instance = new RunnerFrmHelper();
                        }
                    }
                }
                return _instance;
            }
        }
        private RunnerFrmHelper()
        {
        }
        #endregion



        public Form GetAppForm()
        {
            try
            {
                return getAppForm();
            }
            catch (Exception ex)
            {
            }
            return null;
        }

        private Form getAppForm()
        {
            try
            {
                return getRunnerMainFrm();
            }
            catch (Exception ex)
            {
                sendMsg($"界面初始化异常，请确认系统配置！>>\r\n" + ex.ToString());
                return null;
            }
        }


        private string getRunnerdllName()
        {
            var nsName = this.GetType().Namespace;
            var result = nsName.Substring(0, nsName.IndexOf('.')) + ".*" + nsName.Substring(nsName.IndexOf('.'));
            return result;
        }
        private void runMainFrm()
        {
            var typeName = "XNetCore.AppTest.Runner.MainFrm,XNetCore.AppTest.Runner";
            var type = Type.GetType(typeName);
            var runFrm = Activator.CreateInstance(type, null) as Form;
            //return runFrm;
            Application.Run(runFrm);
        }
        private Form getRunnerMainFrm()
        {
            sendMsg("初始化系统运行界面……");
            var runnerFiles = getRunnerFiles();
            if (runnerFiles == null || runnerFiles.Length == 0)
            {
                sendMsg($"{getRunnerdllName()}文件获取失败");
                return null;
            }
            Form frm = null;
            try
            {
                frm = getRunnerForm(runnerFiles);
            }
            catch (Exception ex)
            {
                sendMsg($"界面构造异常，请确认系统配置！>>\r\n{ex.ToString()}");
                return null;
            }
            if (frm == null)
            {
                sendMsg($"未发现系统界面，请确认系统配置！");
                return null;
            }
            sendMsg($"AppPath：{Process.GetCurrentProcess().MainModule.FileName}");
            return frm;
        }

        private void sendMsg(string msg)
        {
            StartLogFileHelper.Instance.StartMsg(msg);
        }


        private Type getFormType(FileInfo runnerFile)
        {
            try
            {
                var ass = Assembly.LoadFile(runnerFile.FullName);
                var ts = ass.GetTypes();
                foreach (var t in ts)
                {
                    if (t.Name.ToLower() == "mainfrm"
                        && typeof(Form).IsAssignableFrom(t))
                    {
                        return t;
                    }
                }
            }
            catch (Exception ex)
            {
                sendMsg($"获取界面类异常>>\r\n" + ex.ToString());
            }
            return null;
        }


        private FileInfo[] getRunnerFiles()
        {
            var result = new List<FileInfo>();
            var thisFile = new FileInfo(new Uri(this.GetType().Assembly.CodeBase).LocalPath);
            var thisfilename = thisFile.Name.ToLower();
            var startName = thisfilename.Substring(0, thisfilename.IndexOf(".") + 1);//techstar.
            var endName = thisfilename.Substring(thisfilename.IndexOf("."));//.runner.dll
            foreach (var dir in PrivatePathHelper.Instance.PrivatePaths)
            {
                var fs = dir.GetFiles();
                foreach (var fi in fs)
                {
                    var fileName = fi.Name.ToLower();
                    if (fileName == thisfilename)
                    {
                        continue;
                    }
                    if (fileName.StartsWith(startName)
                        && fileName.EndsWith(endName)
                        && (!fileName.Equals(thisfilename)))
                    {
                        sendMsg($"Runner文件[{fi.FullName}]");
                        result.Add(fi);
                    }
                }
            }
            return result.ToArray();
        }


        private Form getRunnerForm(FileInfo[] files)
        {
            var frms = getRunnerForms(files);
            if(frms==null || frms.Length==0)
            {
                return null;
            }
            return frms.FirstOrDefault();

        }


        private Form[] getRunnerForms(FileInfo[] files)
        {
            var result = new List<Form>();
            foreach (var file in files)
            {
                var frm = getRunnerForm(file);
                if (frm == null)
                {
                    continue;
                }
                result.Add(frm);
            }
            return result.ToArray();
        }



        private Form getRunnerForm(FileInfo runnerFile)
        {
            sendMsg($"{getRunnerdllName()}信息=" + runnerFile.FullName);
            var type = getFormType(runnerFile);
            if (type == null)
            {
                sendMsg("界面类为空！");
                return null;
            }
            type = Type.GetType(type.AssemblyQualifiedName);
            sendMsg($"界面类[{type.AssemblyQualifiedName}]");
            var obj = System.Activator.CreateInstance(type, null);
            if (obj == null)
            {
                sendMsg($"界面实例创建失败！[{type.AssemblyQualifiedName}]");
                return null;
            }
            var frm = obj as Form;
            if (frm == null)
            {
                sendMsg($"实例无法转为界面！[{type.AssemblyQualifiedName}]");
                return null;
            }
            return frm;
        }






    }
}
