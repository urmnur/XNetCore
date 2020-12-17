using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace XNetCore.KApp
{
    public partial class MainFrm : Form
    {
        #region Initialize

        const int const_sleep_total_seconds = 60;
        public MainFrm()
        {
            InitializeComponent();
            initializeForm();
            initializeFormEvent();
        }
        private void initializeForm()
        {
            this.Text = "应用在线助手";
            this.WindowState = FormWindowState.Maximized;
            this.panel1.Dock = DockStyle.Fill;
            if (KAppHelper.Instance.AppPath != null)
            {
                this.toolStripStatusLabel3.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
                this.toolStripStatusLabel1.Text = KAppHelper.Instance.AppPath.FullName;
                this.textBox1.Text = KAppHelper.Instance.AppPath.FullName;
                this.textBox2.Text = Path.Combine(KAppHelper.Instance.AppPath.Directory.FullName, LocalPath.FirstName + "." + button3.Text);
            }
            this.toolStripButton2.Text = $"关闭 [{this.Text}]";
            this.toolStripButton4.Text = $"{const_sleep_total_seconds / 60}分钟重启";
            sameClieckEvent(this.重启1分钟ToolStripMenuItem, this.toolStripButton4);
            sameClieckEvent(this.重启应用ToolStripMenuItem, this.toolStripButton1);
            sameClieckEvent(this.关闭系统ToolStripMenuItem, this.toolStripButton2);
            sameClieckEvent(this.启动应用ToolStripMenuItem, this.toolStripButton5);

            setFormState(this);
            this.Shown += MainFrm_Shown;
        }


        private void sameClieckEvent(ToolStripMenuItem menuItem, ToolStripButton stripButton)
        {
            menuItem.Text = stripButton.Text;
            menuItem.Click += (sender, e) => { stripButton.PerformClick(); };
        }


        private void MainFrm_Shown(object sender, EventArgs e)
        {
            subscribeAppMsg();
            threadInitialize();
        }

        private void threadInitialize()
        {
            Task.Run(new Action(() =>
            {
                runArgs();
                while (true)
                {
                    try
                    {
                        Thread.Sleep(5 * 1000);
                        showRunTime();
                        if ((DateTime.Now - this.keepalive_time).TotalSeconds < const_sleep_total_seconds)
                        {
                            continue;
                        }
                        KeepAliveHelper.Instance.KeepAlive();
                    }
                    catch (Exception ex)
                    {

                    }
                }
            }));
        }
        private void showRunTime()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => { showRunTime(); }));
                return;
            }
            if (this.keepalive_time == DateTime.MaxValue)
            {
                this.toolStripStatusLabel3.Text = this.keepalive_time.ToString("yyyy-MM-dd HH:mm:ss.fff");
                return;
            }
            if ((DateTime.Now - this.keepalive_time).TotalSeconds < const_sleep_total_seconds)
            {
                this.toolStripStatusLabel3.Text = this.keepalive_time.AddSeconds(const_sleep_total_seconds).ToString("yyyy-MM-dd HH:mm:ss.fff");
                return;
            }
            this.toolStripStatusLabel3.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");

        }

        private void runArgs()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(runArgs));
                return;
            }
            if (KAppHelper.Instance.AutoClose)
            {
                this.Hide();
                return;
            }
            KillAppHelper.Instance.KillApp(
                KAppHelper.Instance.KillAppType,
                KAppHelper.Instance.AppPath
                );
            thisClose();
        }

        private void setFormState(Form frm)
        {
            if (frm == null)
            {
                return;
            }
            frm.WindowState = FormWindowState.Maximized;
            frm.Icon = getfavicon();
        }
        private Icon getfavicon()
        {
            var appFile = new FileInfo(new Uri(Assembly.GetEntryAssembly().CodeBase).LocalPath);
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
            return Icon.ExtractAssociatedIcon(new Uri(Assembly.GetEntryAssembly().CodeBase).LocalPath);
        }

        #endregion

        #region subscribeAppMsg
        private void subscribeAppMsg()
        {
            XMQSession.Current.Subscribe(string.Empty, receiveAppMsg);
        }
        private void receiveAppMsg(string topic, XMessage msg)
        {
            if (msg == null)
            {
                return;
            }
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => { receiveAppMsg(topic, msg); }));
                return;
            }
            var msgdata = formatAppMsg(topic, msg);
            var sb = new StringBuilder();
            sb.Append(this.tbMsg.Text).AppendLine(string.Empty);
            if (sb.Length > 1000 * 100)
            {
                sb.Remove(0, sb.Length - 1000);
            }
            sb.Append(msgdata).AppendLine(string.Empty);
            this.tbMsg.Text = sb.ToString();
            this.tbMsg.SelectionStart = this.tbMsg.Text.Length;
            this.tbMsg.ScrollToCaret();
        }

        private string formatAppMsg(string topic, XMessage msg)
        {
            var result = new StringBuilder();
            var data = string.Empty;
            if (msg.MsgData != null)
            {
                data = msg.MsgData.ToString();
            }
            result.Append($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")} >>> ");
            if (!string.IsNullOrWhiteSpace(msg.MsgTopic))
                result.Append($"[{msg.MsgTopic}]@");
            if (!string.IsNullOrWhiteSpace(msg.MsgMethod))
                result.Append($"[{msg.MsgMethod}]");
            if (!string.IsNullOrWhiteSpace(data))
                result.Append($" >>> \r\n{data}");
            result.AppendLine(string.Empty);
            return result.ToString();
        }
        #endregion

        #region toolStripButton
        DateTime keepalive_time = DateTime.MinValue;
        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            if (KAppHelper.Instance.AppPath == null)
            {
                return;
            }
            var msg = $"{toolStripButton1.Text}  [{KAppHelper.Instance.AppPath.FullName}]";
            if (MessageBox.Show($"确定{ msg }？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Error) != DialogResult.Yes)
            {
                return;
            }
            this.keepalive_time = DateTime.MinValue;
            XMQSession.Current.Publish(new XMessage() { MsgData = msg });
            KillAppHelper.Instance.KillApp(KillAppType.AppSelf, KAppHelper.Instance.AppPath);
        }
        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            if (KAppHelper.Instance.AppPath == null)
            {
                return;
            }
            var msg = $"{toolStripButton4.Text}  [{KAppHelper.Instance.AppPath.FullName}]";
            if (MessageBox.Show($"确定{ msg }？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Error) != DialogResult.Yes)
            {
                return;
            }
            this.keepalive_time = DateTime.Now;
            XMQSession.Current.Publish(new XMessage() { MsgData = msg });
            KillAppHelper.Instance.KillApp(KillAppType.AppSelf, KAppHelper.Instance.AppPath);
        }

        private void toolStripButton5_Click(object sender, EventArgs e)
        {
            if (KAppHelper.Instance.AppPath == null)
            {
                return;
            }
            var msg = $"{toolStripButton5.Text}  [{KAppHelper.Instance.AppPath.FullName}]";
            if (MessageBox.Show($"确定{ msg }？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Error) != DialogResult.Yes)
            {
                return;
            }
            this.keepalive_time = DateTime.MinValue;
            XMQSession.Current.Publish(new XMessage() { MsgData = msg });
        }


        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            if (this.closeApp)
            {
                thisClose();
                return;
            }
            var msg = $"{toolStripButton2.Text}";
            if (MessageBox.Show($"确定{ msg }？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Error) != DialogResult.Yes)
            {
                return;
            }
            XMQSession.Current.Publish(new XMessage() { MsgData = msg });
            thisClose();
        }
        private void thisClose()
        {
            this.closeApp = true;
            this.Close();
            try { Process.GetCurrentProcess().Kill(); } catch { }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (KAppHelper.Instance.AppPath == null)
            {
                return;
            }
            System.Diagnostics.Process.Start(KAppHelper.Instance.AppPath.Directory.FullName);
        }
        #endregion

        #region initializeFormEvent
        private void initializeFormEvent()
        {
            this.Shown += Notify_MainFrm_Shown;
            this.FormClosing += Notify_MainFrm_FormClosing;
        }

        private void Notify_MainFrm_Shown(object sender, EventArgs e)
        {
            this.notifyIcon1.Icon = this.Icon;
            this.notifyIcon1.Text = this.Text;
        }

        private bool closeApp = false;
        private void Notify_MainFrm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.closeApp)
            {
                return;
            }
            e.Cancel = true;
            this.Hide();
        }
        #endregion

        private void 显示主界面ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(this.textBox2.Text))
            {
                return;
            }
            var dir = new DirectoryInfo(this.textBox2.Text);
            if (!dir.Exists)
            {
                dir.Create();
            }
            System.Diagnostics.Process.Start(dir.FullName);

        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(this.textBox2.Text))
            {
                return;
            }
            var dir = new DirectoryInfo(this.textBox2.Text);
            if (!dir.Exists)
            {
                dir.Create();
            }
            if (KAppHelper.Instance.AppPath == null)
            {
                return;
            }


            if (KAppHelper.Instance.AppPath == null)
            {
                return;
            }
            var msg = $" *关闭* 应用进行 *更新* 后重新 *启动* 应用\r\n[{KAppHelper.Instance.AppPath.FullName}]";
            if (MessageBox.Show($"请确认{msg}", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Error) != DialogResult.Yes)
            {
                return;
            }
            this.keepalive_time = DateTime.Now;
            XMQSession.Current.Publish(new XMessage() { MsgData = msg });
            KillAppHelper.Instance.KillApp(KillAppType.AppSelf, KAppHelper.Instance.AppPath);
            SyncFileHelper.Instance.SynUpdateFile(dir, KAppHelper.Instance.AppPath.Directory);
            this.keepalive_time = DateTime.MinValue;
        }
    }
}
