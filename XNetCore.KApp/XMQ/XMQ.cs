using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace XNetCore.KApp
{
    public class XMQSession
    {
        #region 单例模式
        private static object lockobject = new object();
        private static XMQSession _instance = null;
        public static XMQSession Current
        {
            get
            {
                if (_instance == null)
                {
                    lock (lockobject)
                    {
                        if (_instance == null)
                        {
                            _instance = new XMQSession();
                        }
                    }
                }
                return _instance;
            }
        }
        private XMQSession()
        {
        }
        #endregion

        private string getMsgMethodName(string methodName)
        {
            var frameIndex = 2;
            var result = string.Empty;
            var st = new StackTrace(true);
            var sf = st.GetFrame(frameIndex);
            var method = sf.GetMethod();
            try
            {
                result = method.DeclaringType.FullName + "." + method.Name;

                if (!string.IsNullOrWhiteSpace(methodName))
                {
                    result = $"[{methodName}] {result}";
                }
            }
            catch { }
            return result;
        }
        public void Publish(XMessage msg)
        {
            try
            {
                if (msg == null)
                {
                    msg = new XMessage();
                }
                msg.MsgMethod = getMsgMethodName(msg.MsgMethod);
                if(string.IsNullOrWhiteSpace(msg.MsgTopic))
                {
                    msg.MsgTopic = msg.MsgMethod;
                }
                receiveeMsg(msg);
            }
            catch (Exception ex)
            {

            }
        }

        private void receiveeMsg(XMessage msg)
        {
            foreach (var tc in this.topiccallbacks)
            {
                try
                {
                    receiveeMsg(tc, msg);
                }
                catch (Exception ex)
                {

                }
            }
        }

        private void receiveeMsg(TopicCallback tc, XMessage msg)
        {
            if (!isSubscribed(tc.Topic, msg.MsgTopic))
            {
                return;
            }
            if (!isSubscribed(tc.Method, msg.MsgMethod))
            {
                return;
            }
            tc.Callback?.Invoke(tc.Topic, msg);
        }

        private bool isSubscribed(string sub, string msg)
        {
            if (string.IsNullOrWhiteSpace(sub))
            {
                return true;
            }
            if (string.IsNullOrWhiteSpace(msg))
            {
                return false;
            }
            if (msg.ToLower().StartsWith(sub.ToLower()))
            {
                return true;
            }
            return false;
        }


        private class TopicCallback
        {
            public string Topic { get; set; }
            public string Method { get; set; }
            public Action<string, XMessage> Callback { get; set; }
        }

        private List<TopicCallback> topiccallbacks = new List<TopicCallback>();

        public void Subscribe(string topic, Action<string, XMessage> callback)
        {
            this.Subscribe(topic, string.Empty, callback);
        }
        public void Subscribe(string topic,string method, Action<string, XMessage> callback)
        {
            if (callback == null)
            {
                return;
            }
            this.topiccallbacks.Add(new TopicCallback()
            {
                Topic = topic,
                Method = method,
                Callback = callback

            });
        }

    }

    public class XMessage
    {
        public string MsgMethod { get; internal set; }
        public string MsgTopic { get; set; }
        public object MsgData { get; set; }
    }
}