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
    class LocalSubFrmHelper
    {
        #region 单例模式
        private static readonly object lockobject = new object();
        private static LocalSubFrmHelper _instance = null;
        public static LocalSubFrmHelper Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (lockobject)
                    {
                        if (_instance == null)
                        {
                            _instance = new LocalSubFrmHelper();
                        }
                    }
                }
                return _instance;
            }
        }
        private LocalSubFrmHelper()
        {
        }
        #endregion

        private void sendMsg(string msg)
        {

            StartLogFileHelper.Instance.StartMsg(msg);
        }


        private FileInfo[] getRunnerFiles()
        {
            var result = new List<FileInfo>();
            var thisFile = new FileInfo(new Uri(this.GetType().Assembly.CodeBase).LocalPath);
            var thisfilename = thisFile.Name.ToLower();
            var startName = thisfilename.Substring(0, thisfilename.IndexOf(".") + 1);//XNetCore.
            var endName = thisfilename.Substring(thisfilename.LastIndexOf("."));//.dll
            var local = new FileInfo(new Uri(this.GetType().Assembly.CodeBase).LocalPath).Directory;
            local = new DirectoryInfo(Path.Combine(local.FullName, this.GetType().Namespace));
            if(!local.Exists)
            {
                return result.ToArray();
            }
            var runner = new FileInfo(Path.Combine(local.FullName, this.GetType().Namespace));
            if (!runner.Exists)
            {
                return result.ToArray();
            }
            foreach (var fi in local.GetFiles())
            {
                var fileName = fi.Name.ToLower();
                if (fileName == thisFile.Name.ToLower())
                {
                    continue;
                }
                if (fileName.StartsWith(startName)
                    && fileName.EndsWith(endName))
                {
                    result.Add(fi);
                }
            }
            return result.ToArray();
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

        private Form getForm(FileInfo file)
        {
            var type = getFormType(file);
            if (type == null)
            {
                sendMsg("界面类为空！");
                return null;
            }
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

        public Form GetAppForm()
        {
            sendMsg($"获取子路径中类库的界面……");
            var files = getRunnerFiles();
            foreach (var file in files)
            {
                sendMsg($"{file.Name}>>MainFrm");
                var frm = getForm(file);
                if (frm != null)
                {
                    sendMsg($"获取子路径中类库成功");
                    return frm;
                }
            }
            sendMsg($"获取子路径中类库失败");
            return null;
        }
    }
}
