using System;
using System.Diagnostics;
using System.Windows.Forms;
using XNetCore.CEF.Runner;
using XNetCore.STL;
using XNetCore.XRun;

namespace XNetCore.CEF.Runner
{
    public partial class MainFrm : Form
    {
        internal static Form CefMainFrm { get; private set; }
        public MainFrm()
        {
            InitializeComponent();
            initializeFormEvent();
            initializeButoonText();
            IniFormIconTitle();
            CefMainFrm = this;
        }

        #region Icon Title
        private void IniFormIconTitle()
        {
            setAddress(string.Empty);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            //this.FormClosed += (sender, e) => { ClientHelper.Instance.Shutdown(); };
            ClientHelper.Instance.AddressChanged += (address) => { try { setAddress(address); } catch { }; };
            this.Shown += MainFrm_Shown;
            this.TextChanged += (sender, e) => { try { this.notifyIcon1.Text = this.Text; } catch (Exception ex) { }; };
        }


        private void MainFrm_Shown(object sender, EventArgs e)
        {
            
            this.notifyIcon1.Icon = ClientHelper.Instance.favicon;
           
            IniClientControl();
            Notify_MainFrm_Shown(sender, e);
            MainFrm_Maximized(sender, e);
        }

       
        private void IniClientControl()
        {
            var control = getClientControl();
            control.TextChanged += (sender, e) => { try { this.onTextChanged(sender, e); } catch (Exception ex) { }; };
            control.Dock = DockStyle.Fill;
            this.panel.Controls.Add(control);
            this.Text = control.Text;
            return;
        }

        private Control getClientControl()
        {
            return ClientHelper.Instance.MainFrmControl;
        }
        private void onTextChanged(object sender, EventArgs e)
        {
            if (sender == null)
            {
                return;
            }
            if (this.InvokeRequired)
            {
                this.Invoke(new Action<object, EventArgs>(onTextChanged), sender, e);
                return;
            }
            var c = sender as Control;
            if (c == null)
            {
                return;
            }
            this.Text = c.Text;
        }


        private void setAddress(string address)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action<string>(setAddress), address);
                return;
            }
        }
        #endregion

        #region initializeFormEvent
        private void initializeFormEvent()
        {
            //this.Shown += Notify_MainFrm_Shown;
            this.FormClosing += Notify_MainFrm_FormClosing;
        }

        private void Notify_MainFrm_Shown(object sender, EventArgs e)
        {
            this.notifyIcon1.Icon = this.Icon;
            this.notifyIcon1.Text = this.Text;
            this.notifyIcon1.Visible = ConfigHelper.Instance.AppDomain.ShowNotify;
            this.closeApp = !this.notifyIcon1.Visible;
        }
        private void MainFrm_Maximized(object sender, EventArgs e)
        {
            if (ConfigHelper.Instance.AppDomain.Maximized)
            {
                this.FormBorderStyle = FormBorderStyle.None;
                this.WindowState = FormWindowState.Maximized;
            }
        }

        private void Notify_MainFrm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (ClientHelper.Instance.ApplicationExit)
            {
                return;
            }
            if (!string.IsNullOrWhiteSpace(ConfigHelper.Instance.AppDomain.JsClose))
            {
                var jsResult =new JsRunner().ExecuteScript(ConfigHelper.Instance.AppDomain.JsClose);
                if (jsResult != null)
                {
                    if (jsResult.ToString().ToInt(0) == 1)
                    {
                        e.Cancel = true;
                    }
                }
                return;
            }
            if (this.closeApp)
            {
                return;
            }
            e.Cancel = true;
            this.Hide();
        }
        #endregion

        #region initializeContent
        private void initializeButoonText()
        {
            this.显示主界面ToolStripMenuItem.Text = $"显示主界面";
            this.关闭系统ToolStripMenuItem.Text = $"关闭系统";
        }
        #endregion

        #region initializeNotify

        private void thisClose()
        {
            this.closeApp = true;
            try { this.Close(); } catch { }
            return;
            try { Process.GetCurrentProcess().Kill(); } catch { }
            try { Application.Exit(); } catch { }
        }

        private void 显示主界面ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Show();
        }
        public bool closeApp = false;
        private void 关闭系统ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.closeApp)
            {
                thisClose();
                return;
            }
            if (MessageBox.Show($"确定{ this.关闭系统ToolStripMenuItem.Text }吗？？？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Error) != DialogResult.Yes)
            {
                return;
            }
            thisClose();
        }
        #endregion

    }
}
