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
    class FormStateHelper
    {
        #region 单例模式
        private static readonly object lockobject = new object();
        private static FormStateHelper _instance = null;
        public static FormStateHelper Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (lockobject)
                    {
                        if (_instance == null)
                        {
                            _instance = new FormStateHelper();
                        }
                    }
                }
                return _instance;
            }
        }
        private FormStateHelper()
        {
            setCurrentFormState();
        }
        #endregion

        public void SetFormState(Form frm)
        {
            if (frm==null)
            {
                return;
            }
            setFormState(frm);
        }

        private void setCurrentFormState()
        {
            setFormState(SysLogFrm.Instance);
        }


        private void setFormState(Form frm)
        {
            if (frm == null)
            {
                return;
            }
            frm.Icon = RunHelper.Instance.AppIcon;
            frm.WindowState = FormWindowState.Maximized;
        }
        private Icon getfavicon()
        {
            var appFile = new FileInfo(Process.GetCurrentProcess().MainModule.FileName);
            if (!appFile.Exists)
            {
                return null;
            }
            var faviconFile = new FileInfo(Path.Combine(appFile.Directory.FullName, "favicon.ico"));
            if (faviconFile.Exists)
            {
                try
                {
                    var stream = new FileStream(faviconFile.FullName, FileMode.Open, FileAccess.Read);
                    Bitmap favicon = new Bitmap(stream);
                    stream.Close();
                    stream.Dispose();
                    return Icon.FromHandle(favicon.GetHicon());
                }
                catch { };
            }
            return Icon.ExtractAssociatedIcon(appFile.FullName);
        }

    }
}
