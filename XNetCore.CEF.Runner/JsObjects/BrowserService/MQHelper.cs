using CefSharp;
using CefSharp.WinForms;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using XNetCore.STL;

namespace XNetCore.CEF.Runner
{
    class MQHelper
    {
        #region 单例模式
        private static object lockobject = new object();
        private static MQHelper _instance = null;
        public static MQHelper Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (lockobject)
                    {
                        if (_instance == null)
                        {
                            _instance = new MQHelper();
                        }
                    }

                }
                return _instance;
            }
        }
        private MQHelper()
        {
        }
        #endregion

        public void ClearMQSubscribe(string url)
        {
            foreach (var topiccallback in this.topiccallbacks.Values)
            {
                if (string.IsNullOrWhiteSpace(url))
                {
                    mRemoveMQSubscribe(topiccallback.XGuid);
                }
                if (string.IsNullOrWhiteSpace(topiccallback.Url))
                {
                    continue;
                }
                if (topiccallback.Url.ToLower().StartsWith(url.ToLower()))
                {
                    mRemoveMQSubscribe(topiccallback.XGuid);
                }

            }
        }

        private void mRemoveMQSubscribe(string xguid)
        {
            topiccallbacks.TryRemove(xguid, out var value);
            XNetCore.STL.XMQ.Current.Remove(xguid);
        }

        public void MQSubscribe(string url, string topic, string method, string callback)
        {
            lock(lockobject)
            {
                removeMQSubscribe(url, topic, method, callback);
                mMQSubscribe(url, topic, method, callback);
            }
        }
        private void mMQSubscribe(string url, string topic, string method, string callback)
        {
            if (string.IsNullOrWhiteSpace(callback))
            {
                return;
            }
            var subscriber = getXSubscriber(topic, method);

            var frame = getFrame(url);
            if (frame == null)
            {
                frame = WebBrowserHelper.Instance.WebBrowser.GetFocusedFrame();
            }
            if (frame == null)
            {
                frame = WebBrowserHelper.Instance.WebBrowser.GetMainFrame();
            }
            topiccallbacks.TryAdd(subscriber.XGuid,
                new FrameSubscribe
                {
                    XGuid = subscriber.XGuid,
                    XSubscriber = subscriber,
                    Url = url,
                    JsCallback = callback,
                    Frame = frame,
                }
            );
            XNetCore.STL.XMQ.Current.Subscribe(subscriber);
        }

        private void removeMQSubscribe(string url, string topic, string method, string callback)
        {
            foreach(var d in topiccallbacks.Values)
            {
                if (d.XSubscriber==null)
                {
                    mRemoveMQSubscribe(d.XGuid);
                }
                if (d.Url.Null2Empty().ToLower()==url.Null2Empty().ToLower()
                    && d.XSubscriber.Topic.Null2Empty().ToLower() == topic.Null2Empty().ToLower()
                    && d.XSubscriber.Method.Null2Empty().ToLower() == method.Null2Empty().ToLower()
                    && d.JsCallback.Null2Empty().ToLower() == callback.Null2Empty().ToLower()
                    )
                {
                    mRemoveMQSubscribe(d.XGuid);
                }
            }
        }




        private ConcurrentDictionary<string, FrameSubscribe> topiccallbacks = new ConcurrentDictionary<string, FrameSubscribe>(StringComparer.OrdinalIgnoreCase);


        class FrameSubscribe
        {
            public string XGuid { get; set; }
            public XSubscriber XSubscriber { get; set; }
            public string Url { get; set; }
            public string JsCallback { get; set; }
            public IFrame Frame { get; set; }
        }


        private IFrame getFrame(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                return null;
            }
            var browser = WebBrowserHelper.Instance.WebBrowser.GetBrowser();
            var names = browser.GetFrameNames();
            IFrame result = null;
            foreach (var name in names)
            {
                var frame = browser.GetFrame(name);
                if (frame == null)
                {
                    continue;
                }
                if (string.IsNullOrWhiteSpace(frame.Url))
                {
                    continue;
                }
                if (frame.Url.ToLower() == url.ToLower())
                {
                    return frame;
                }
                if (frame.Url.ToLower().StartsWith(url.ToLower()))
                {
                    result = frame;
                }
            }
            return result;
        }

        private XSubscriber getXSubscriber(string topic, string method)
        {
            var result = new XSubscriber();
            result.Topic = topic;
            result.Method = method;
            result.Callback = receiveAppMsg;
            return result;
        }
        JsRunner jsRunner = new JsRunner();
        private void receiveAppMsg(XSubscriber subscriber, XMessage msg)
        {
            try
            {
                if (!topiccallbacks.TryGetValue(subscriber.XGuid, out FrameSubscribe callback))
                {
                    return;
                }
                if (callback == null || callback.Frame == null || string.IsNullOrWhiteSpace(callback.JsCallback))
                {
                    return;
                }
                mExecuteScriptBase64Async(callback, msg);
            }
            catch
            { }
        }

        private void mExecuteScriptBase64Async(FrameSubscribe callback, object args)
        {
            var msg = args.ToIndentedJson();
            var script = new StringBuilder();
            script.Append(callback.JsCallback).Append($"('{Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(msg))}');").AppendLine(string.Empty);
            callback.Frame.ExecuteJavaScriptAsync(script.ToString());
        }
    }
    class XMQ
    {
        public void MQSubscribe(string url, string topic, string method, string callback)
        {
            MQHelper.Instance.MQSubscribe(url, topic, method, callback);
        }
        public void ClearMQSubscribe(string url)
        {
            MQHelper.Instance.ClearMQSubscribe(url);
        }
    }
}