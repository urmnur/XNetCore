using CefSharp;
using CefSharp.WinForms;
using System;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using XNetCore.STL;

namespace XNetCore.CEF.Runner
{
    class CefApp
    {
        public void ShowDevTools()
        {
            var mainbrowser = WebBrowserHelper.Instance.WebBrowser;
            if (mainbrowser == null)
            {
                return;
            }
            mainbrowser.ShowDevTools();
        }
        public void Close()
        {
            var frm = MainFrm.CefMainFrm;
            if (frm.InvokeRequired)
            {
                frm.BeginInvoke(new Action(Close));
                return;
            }
            ClientHelper.Instance.ApplicationExit = true;
            try
            {
                var mainbrowser = WebBrowserHelper.Instance.WebBrowser;
                if (mainbrowser == null)
                {
                    return;
                }
                mainbrowser.CloseDevTools();
            }
            catch { }
            frm.Close();
        }
        private int mainFormBorderStyle = -1;
        public void FullScreen()
        {
            var frm = MainFrm.CefMainFrm;
            if (frm.InvokeRequired)
            {
                frm.BeginInvoke(new Action(FullScreen));
                return;
            }
            frm.TopMost = false;
            frm.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            if (mainFormBorderStyle < 0)
            {
                mainFormBorderStyle = (int)frm.FormBorderStyle;
                if (frm.FormBorderStyle == FormBorderStyle.None)
                {
                    mainFormBorderStyle = (int)FormBorderStyle.Sizable;
                }
            }
            if (frm.FormBorderStyle != FormBorderStyle.None)
            {
                frm.FormBorderStyle = FormBorderStyle.None;
                frm.WindowState = FormWindowState.Maximized;
            }
            else
            {
                frm.FormBorderStyle = (FormBorderStyle)mainFormBorderStyle;
            }
        }
    }
}