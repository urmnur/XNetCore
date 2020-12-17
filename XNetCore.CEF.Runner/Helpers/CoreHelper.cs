using CefSharp;
using CefSharp.WinForms;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using XNetCore.STL;

namespace XNetCore.CEF.Runner
{
    class CoreHelper
    {
        #region 单例模式
        private static object lockobject = new object();
        private static CoreHelper _instance = null;
        public static CoreHelper Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (lockobject)
                    {
                        if (_instance == null)
                        {
                            _instance = new CoreHelper();
                        }
                    }

                }
                return _instance;
            }
        }

        private CoreHelper()
        {
            IniClient();
        }
        #endregion



        private void IniClient()
        {
            RunApiService();
        }
        public Control MainFrmControl { get { return getMainFrmControl(); } }


        private Control __MainFrmControl = null;
        private Control getMainFrmControl()
        {
            if (this.__MainFrmControl != null)
            {
                return this.__MainFrmControl;
            }
            this.__MainFrmControl = getMainControl();
            return this.__MainFrmControl;
        }


        private Control getMainControl()
        {
            return getWebBrowser();
        }
        private WebBrowser __webBrowser = null;
        private WebBrowser getWebBrowser()
        {
            if (this.__webBrowser != null)
            {
                return this.__webBrowser;
            }
            this.__webBrowser = WebBrowserHelper.Instance.WebBrowser;
            iniWebBrowserEventH(this.__webBrowser);
            return this.__webBrowser;
        }

        private void RunApiService()
        {
            if (ConfigHelper.Instance.AppDomain?.ApiPort > 0)
            {
                new XNetCore.XAPI.XApi().Start(ConfigHelper.Instance.AppDomain.ApiPort);
            }
            if (ConfigHelper.Instance.AppDomain?.RpcPort > 0)
            {
                new XNetCore.RPC.Server.RpcServer().Start(ConfigHelper.Instance.AppDomain.RpcPort);
            }
        }


        public event Action<string> AddressChanged;
        private void iniWebBrowserEventH(WebBrowser browser)
        {
            if (browser == null)
            {
                return;
            }
            browser.TitleChanged += (sender, e) => { try { this.onWebBrowserTitleChanged(sender, e); } catch (Exception ex) { }; };
            browser.AddressChanged += (sender, e) => { try { this.AddressChanged?.Invoke(e.Address); } catch { }; };
        }
        private void onWebBrowserTitleChanged(object sender, TitleChangedEventArgs e)
        {

            if (sender == null)
            {
                return;
            }
            var browser = sender as WebBrowser;
            if (browser == null)
            {
                return;
            }
            Control p = browser;
            while (true)
            {
                p = p.Parent;
                if (p == null)
                {
                    return;
                }
                if (setParentFormText(p as Form, e.Title) > 0)
                {
                    return;
                }
            }
        }


        private int setParentFormText(Form d, string text)
        {
            if (d == null)
            {
                return 0;
            }
            if (d.InvokeRequired)
            {
                d.Invoke(new Action<string>(t => { d.Text = t; }), text);
                return 1;
            }
            d.Text = text;
            return 1;
        }


        public Icon favicon { get { return getfavicon(); } }

        private Icon getfavicon()
        {
            var result = gethttpfavicon();
            if (result != null)
            {
                return result;
            }
            var domainName = DomainHelper.Instance.GetName(typeof(CDNScheme));
            result = getresourcefavicon(domainName);
            if (result != null)
            {
                return result;
            }
            domainName = DomainHelper.Instance.GetName(typeof(UIScheme));
            result = getresourcefavicon(domainName);
            if (result != null)
            {
                return result;
            }
            return result;
        }

        private Icon getresourcefavicon(string domainName)
        {

            var buff = PakHelper.Instance.GetResource(domainName, "favicon.ico");
            if (buff.Length > 0)
            {
                try
                {
                    var stream = new MemoryStream();
                    stream.Write(buff, 0, buff.Length);
                    Bitmap iconBm = new Bitmap(stream);
                    stream.Close();
                    stream.Dispose();
                    return Icon.FromHandle(iconBm.GetHicon());
                }
                catch { };
            }
            return null;
        }

        private Icon gethttpfavicon()
        {
            if (this.__webBrowser == null)
            {
                return null;
            }
            try
            {
                var browser = this.__webBrowser as WebBrowser;
                if (browser == null)
                {
                    return null;
                }
                var url = browser.Address;
                var idx = -1;
                for (var i = 0; i < 3; i++)
                {
                    idx = url.IndexOf("/", idx + 1);
                    if (idx < 0)
                    {
                        break;
                    }
                }
                if (idx > 0)
                {
                    url = url.Substring(0, idx);
                }
                url = url + "/favicon.ico";
                HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(url);
                if (url.ToLower().StartsWith("https"))
                {
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls;
                }
                req.Timeout = 2000;
                req.Method = "GET";
                req.ContentType = "image/x-icon";
                req.ContentLength = 0;
                using (HttpWebResponse res = (HttpWebResponse)req.GetResponse())
                {

                    Stream resStream = res.GetResponseStream();
                    if (resStream != null)
                    {
                        Bitmap iconBm = new Bitmap(resStream);
                        var result = Icon.FromHandle(iconBm.GetHicon());
                        resStream.Close();
                        return result;
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return null;
        }
    }
}