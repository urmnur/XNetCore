using CefSharp;
using CefSharp.WinForms;
using System;
using System.Text;
using System.Threading.Tasks;
using XNetCore.STL;

namespace XNetCore.CEF.Runner
{
    class JsRunner
    {
        public void ExecuteScriptAsync(string script)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(script))
                {
                    return;
                }
                var mainbrowser = WebBrowserHelper.Instance.WebBrowser;
                if (mainbrowser == null)
                {
                    return;
                }
                mainbrowser.ExecuteScriptAsync(script);
            }
            catch { }
        }


        public object ExecuteScript(string script)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(script))
                {
                    return null;
                }
                var frame = WebBrowserHelper.Instance.WebBrowser.GetMainFrame();
                if (frame == null)
                {
                    return null;
                }
                var task = frame.EvaluateScriptAsync(script);
                task.Wait();
                var result = task.Result.Result;
                return result;
            }
            catch { }
            return null;
        }

    }
}