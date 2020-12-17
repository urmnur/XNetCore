using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;

namespace XNetCore.STL
{
    class XMQSession
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

        private string getMsgMethodName(int pStackFrameIndex)
        {
            var frameIndex = 2 + pStackFrameIndex;
            var result = string.Empty;
            try
            {
                var st = new StackTrace(true);
                var sf = st.GetFrame(frameIndex);
                var method = sf.GetMethod();

                result = method.DeclaringType.FullName + "." + method.Name;
            }
            catch { }
            return result;
        }
        public void Publish(XMessage msg)
        {
            try
            {
                mPublishMsg(msg);
            }
            catch (Exception ex)
            {

            }
        }
        private void xLogMsg(XMessage msg)
        {
            if (msg == null)
            {
                return;
            }
            if (msg.StackFrameIndex > 0)
            {
                msg.MsgMethod = getMsgMethodName(msg.StackFrameIndex + 1);
            }
            else
            {
                msg.MsgMethod = getMsgMethodName(1);
            }

            if (string.IsNullOrWhiteSpace(msg.MsgTopic))
            {
                msg.MsgTopic = msg.MsgMethod;
            }
        }
        private void mPublishMsg(XMessage msg)
        {
            if (msg == null)
            {
                return;
            }
            if (msg.StackFrameIndex > 0)
            {
                msg.MsgMethod = getMsgMethodName(msg.StackFrameIndex + 1);
            }
            else
            {
                msg.MsgMethod = getMsgMethodName(1);
            }

            if (string.IsNullOrWhiteSpace(msg.MsgTopic))
            {
                msg.MsgTopic = msg.MsgMethod;
            }
            foreach (var subscriber in this.topiccallbacks.Values)
            {
                try
                {
                    if ((!isSubscribed(subscriber.Topic, msg.MsgTopic, true))
                         && (!isSubscribed(subscriber.Method, msg.MsgMethod, false)))
                    {
                        continue;
                    }
                    subscriber.Callback?.Invoke(subscriber, msg);
                }
                catch (Exception ex)
                {
                }
            }
        }


        private bool isSubscribed(string subscriber, string msg, bool bnull)
        {
            if (string.IsNullOrWhiteSpace(subscriber))
            {
                return bnull;
            }
            if (string.IsNullOrWhiteSpace(msg))
            {
                return false;
            }
            if (msg.ToLower().StartsWith(subscriber.ToLower()))
            {
                return true;
            }
            if (msg.IsMatch(subscriber, true))
            {
                return true;
            }
            return false;
        }

        private ConcurrentDictionary<string, XSubscriber> topiccallbacks = new ConcurrentDictionary<string, XSubscriber>();

        public void Subscribe(XSubscriber subscriber)
        {
            lock (lockobject)
            {
                mSubscribe(subscriber);
            }
        }
        public void Remove(string xguid)
        {
            lock (lockobject)
            {
                topiccallbacks.TryRemove(xguid, out var value);
            }
        }
        private void mSubscribe(XSubscriber subscriber)
        {
            try
            {
                if (subscriber == null)
                {
                    return;
                }
                var key = $"[{subscriber.Topic}][{subscriber.Method}]".ToLower();
                if (!this.topiccallbacks.ContainsKey(key))
                {
                    this.topiccallbacks.TryAdd(subscriber.XGuid, subscriber);
                }
            }
            catch { }
        }
    }
}