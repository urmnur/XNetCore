using CefSharp;
using CefSharp.WinForms;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace XNetCore.CEF.Runner
{
    class WebBrowser : ChromiumWebBrowser
    {
        private WebBrowser() : this(string.Empty)
        {
        }
        internal WebBrowser(string address) : base(address)
        {
        }
        public new void Dispose()
        {
            try { base.Dispose(); } catch { }
            try { Cef.Shutdown(); } catch { };
        }

    }
}