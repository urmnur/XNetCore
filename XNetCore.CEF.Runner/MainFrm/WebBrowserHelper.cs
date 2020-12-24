using CefSharp;
using CefSharp.WinForms;
using System;
using System.Collections.Generic;
using System.DirectoryServices.ActiveDirectory;
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
    class WebBrowserHelper
    {
        #region 单例模式
        private static object lockobject = new object();
        private static WebBrowserHelper _instance = null;
        public static WebBrowserHelper Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (lockobject)
                    {
                        if (_instance == null)
                        {
                            _instance = new WebBrowserHelper();
                        }
                    }

                }
                return _instance;
            }
        }

        private WebBrowserHelper()
        {
        }
        #endregion

        public WebBrowser WebBrowser { get { return getWebBrowser(); } }


        private WebBrowser __webBrowser = null;
        private WebBrowser getWebBrowser()
        {
            if (this.__webBrowser != null)
            {
                return this.__webBrowser;
            }
            var url = ConfigHelper.Instance.AppDomain.Url;
            if (string.IsNullOrWhiteSpace(url))
            {
                var domainName = DomainHelper.Instance.GetName(typeof(UIScheme));
                url = $"http://{domainName}/index.html";
            }
            if (!url.ToLower().StartsWith("http://") && !url.ToLower().StartsWith("https://"))
            {
                url = "http://" + url;
            }
            IniCefSettings();
            var browser = new WebBrowser(url)
            {
                BrowserSettings =
                    {
                        WebSecurity = CefState.Disabled,
                        DefaultEncoding = "UTF-8",
                        WebGl = CefState.Disabled
                    }
            };
            IniHandler(browser);
            RegisterJsObject(browser);
            browser.Dock = DockStyle.Fill;
            browser.LoadError += Browser_LoadError;
            this.__webBrowser = browser;
            return this.__webBrowser;
        }

        private void Browser_LoadError(object sender, LoadErrorEventArgs e)
        {
            XNetCore.STL.XMQ.Current.Publish(new XMessage()
            {
                MsgTopic = $"Browser_LoadError",
                MsgData = new { MainFrameUrl = e.Browser.MainFrame.Url, FailedUrl = e.FailedUrl, ErrorCode = e.ErrorCode, ErrorText = e.ErrorText }.ToJson()
            });
        }

        private void IniHandler(WebBrowser browser)
        {
            browser.MenuHandler = new ContextMenuHandler();
            browser.KeyboardHandler = new KeyboardHandler();
            browser.LifeSpanHandler = new OpenPageSelfHandler();
            //browser.RequestHandler = new ResourceRequestHandler();
        }


        private void RegisterJsObject(WebBrowser browser)
        {
            var all = JsObjectFactory.Instance.AllJsObjects;
            foreach (var item in all)
            {
                CefSharpSettings.LegacyJavascriptBindingEnabled = true;
                browser.JavascriptObjectRepository.Register(item.Name, item.Instance, isAsync: true, options: BindingOptions.DefaultBinder);
            }
        }


        private FileInfo getBrowserSubprocess()
        {
            foreach (var dir in XNetCore.STL.Runner.Instance.PrivatePaths)
            {
                var fi = new FileInfo(Path.Combine(dir.FullName, "CefSharp.BrowserSubprocess.exe"));
                if (fi.Exists)
                {
                    return fi;
                }
            }
            return null;
        }

        private void IniCefSettings()
        {
            CefSharpSettings.LegacyJavascriptBindingEnabled = true;
            var settings = new CefSettings();
            settings.Locale = "zh-CN";
            settings.AcceptLanguageList = settings.Locale;
            settings.MultiThreadedMessageLoop = true;
            settings.CefCommandLineArgs.Add("enable-media-stream", "1");
            settings.CefCommandLineArgs.Add("no-proxy-server", "1");
            settings.LogSeverity = LogSeverity.Disable;
            var subprocess = getBrowserSubprocess();
            if (subprocess != null)
            {
                settings.BrowserSubprocessPath = subprocess.FullName;
            }
            var cachedir = new DirectoryInfo(Path.Combine(LocalPath.CurrentTempPath.FullName, "cache"));
            if (cachedir.Exists)
            {
                cachedir.Delete(true);
            }
            settings.CachePath = cachedir.FullName;
            var schemes = getCefCustomSchemes();
            foreach (var scheme in schemes)
            {
                settings.RegisterScheme(scheme);
            }
            Cef.Initialize(settings);
            Cef.EnableHighDPISupport();
        }

        private CefCustomScheme[] getCefCustomSchemes()
        {
            var result = new List<CefCustomScheme>();
            var all = CustomSchemeFactory.Instance.AllSchemes;
            foreach (var item in all)
            {
                result.Add(new CefCustomScheme
                {
                    SchemeName = "http",
                    DomainName = item.DomainName.ToLower(),
                    SchemeHandlerFactory = item.Instance,
                    IsSecure = true, //treated with the same security rules as those applied to "https" URLs
                    IsCorsEnabled = false,
                });
            }
            return result.ToArray();
        }



        public void Shutdown()
        {
            try { Cef.Shutdown(); } catch { }
        }
    }
}