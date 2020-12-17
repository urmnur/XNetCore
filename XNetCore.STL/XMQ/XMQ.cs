using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;

namespace XNetCore.STL
{
    public class XMQ
    {
        #region 单例模式
        private static object lockobject = new object();
        private static XMQ _instance = null;
        public static XMQ Current
        {
            get
            {
                if (_instance == null)
                {
                    lock (lockobject)
                    {
                        if (_instance == null)
                        {
                            _instance = new XMQ();
                        }
                    }
                }
                return _instance;
            }
        }
        private XMQ()
        {
        }
        #endregion

        public void Publish(XMessage msg)
        {
            if (msg == null)
            {
                return;
            }
            msg.StackFrameIndex = msg.StackFrameIndex + 1;
            XMQSession.Current.Publish(msg);
        }

        public void Subscribe(XSubscriber subscriber)
        {
            XMQSession.Current.Subscribe(subscriber);
        }
        public void Remove(string xguid)
        {
            XMQSession.Current.Remove(xguid);
        }
    }

    public class XSubscriber
    {
        public XSubscriber()
        {
            this.XGuid = Guid.NewGuid().ToString();
        }
        public string XGuid { get; private set; }
        public string Topic { get; set; }
        public string Method { get; set; }
        public Action<XSubscriber, XMessage> Callback { get; set; }
    }

    public class XMessage
    {
        public XMessage()
        {
            this.MsgGuid = Guid.NewGuid().ToString();
            this.MsgTime = DateTime.Now;
        }
        public string MsgGuid { get; private set; }
        public DateTime MsgTime { get; private set; }
        public string MsgMethod { get; internal set; }
        public int StackFrameIndex { get; set; }
        public string MsgTopic { get; set; }
        public object MsgData { get; set; }
    }

}