using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace XNetCore.Runner
{
    partial class SysLogFrm : Form
    {
        #region 单例模式
        private static readonly object lockobject = new object();
        private static SysLogFrm _instance = null;
        public static SysLogFrm Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (lockobject)
                    {
                        if (_instance == null)
                        {
                            _instance = new SysLogFrm();
                        }
                    }
                }
                return _instance;
            }
        }
        #endregion

        private SysLogFrm()
        {
            InitializeComponent(); ;
            this.FormClosing += SysLogFrm_FormClosing;
            this.Shown += SysLogFrm_Shown;
        }

        private void SysLogFrm_Shown(object sender, EventArgs e)
        {
            this.Icon = XNetCore.Runner.RunHelper.Instance.AppIcon;
            XNetCore.Runner.RunHelper.Instance.IconChanged += (icon) => { this.Icon = icon; };
            if (RunHelper.Instance.MainFrm != null)
            {
                this.Text = $"启动日志[{RunHelper.Instance.MainFrm.Text}]";
            }
            this.textBox4.Text = Process.GetCurrentProcess().MainModule.FileName;
        }

        private void SysLogFrm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this == RunHelper.Instance.MainFrm)
            {
                return;
            }
            this.Hide();
            e.Cancel = true;
        }

        internal void ShowMsg(string msg)
        {
            try
            {
                showSysLog(msg);
            }
            catch (Exception ex)
            {

            }
        }

        private static readonly object lockException = new object();
        private int isException = 0;
        internal void ShowException(Exception err)
        {
            lock (lockException)
            {
                if (isException>0)
                {
                    return;
                }
                isException++;
                try
                {
                    showExceptionForm(err);
                }
                catch (Exception ex)
                {

                }
            }
        }
        private void showExceptionForm(Exception err)
        {
            if (this.tbStart.InvokeRequired)
            {
                this.tbStart.BeginInvoke(new Action(() => { showExceptionForm(err); }));
                return;
            }
            if (err != null)
            {
                showSysLog(err.ToString());
            }
            this.ShowDialog();
            AppKillerHelper.Instance.KillCurrentApp();
        }


        private void showSysLog(string msg)
        {
            if (msg == null || string.IsNullOrWhiteSpace(msg))
            {
                return;
            }
            if (this.tbStart.InvokeRequired)
            {
                this.tbStart.BeginInvoke(new Action(() => { showSysLog(msg); }));
                return;
            }
            if (msg.ToLower() == "clear")
            {
                this.tbStart.Text = string.Empty;
                return;
            }
            var msgdata = $"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")}\r\n      {msg}\r\n\r\n";
            this.tbStart.AppendText(msgdata);
            this.Refresh();
        }


        private const int WM_APP = 0x8000;
        private const int WM_TECHSTAR_FORM = WM_APP + 20;
        private const int TECHSTAR_FORM = 13016;
        internal void ShowMainForm(IntPtr hwnd)
        {
            var flag = SysMenu.SendMessage(hwnd, WM_TECHSTAR_FORM, TECHSTAR_FORM, IntPtr.Zero);
        }
        protected override void WndProc(ref System.Windows.Forms.Message e)
        {
            if (e.Msg == WM_TECHSTAR_FORM)// WM_LBUTTONDOWN
            {
                showMainFrm();
            }
            base.WndProc(ref e);
        }


        private void showMainFrm()
        {
            if (RunHelper.Instance.MainFrm.InvokeRequired)
            {
                RunHelper.Instance.MainFrm.Invoke(new Action(() => { showMainFrm(); }));
                return;
            }
            RunHelper.Instance.MainFrm.Show();
        }


        private void Button1_Click(object sender, EventArgs e)
        {
            var file = new System.IO.FileInfo(textBox4.Text);
            if (!file.Exists)
            {
                return;
            }
            System.Diagnostics.Process.Start(file.Directory.FullName);
        }

    }
}
