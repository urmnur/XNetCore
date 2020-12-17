using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XNetCore.STL;

namespace XNetCore.XAPI
{
    class OptionHelper
    {
        #region 单例模式
        private static object lockobject = new object();
        private static OptionHelper _instance = null;
        public static OptionHelper Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (lockobject)
                    {
                        if (_instance == null)
                        {
                            _instance = new OptionHelper();
                        }
                    }

                }
                return _instance;
            }
        }
        private OptionHelper()
        {
        }
        #endregion


        public async Task Run(HttpContext context, Func<Task> task)
        {
            httpRun(context, task);
        }
        private void httpRun(HttpContext context, Func<Task> task)
        {
            response(context);
            var method = context.Request.Method;
            if (method.ToUpper() != "OPTIONS")
            {
                task.Invoke();
                return;
            }
        }
        private void responseAppendHeader(HttpContext context, string key, string value)
        {
            foreach (var hk in context.Response.Headers.Keys)
            {
                if (hk != null && hk.ToLower() == key.ToLower())
                {
                    context.Response.Headers[hk] = value;
                    return;
                }
            }
            context.Response.Headers.Add(key, value);
        }

        private void response(HttpContext context)
        {
            string origin = context.Request.Headers["Origin"];
            var headers =new XHeader().ResponseHeaders(origin);
            foreach (var kv in headers)
            {
                responseAppendHeader(context, kv.Key, kv.Value);
            }
            context.Response.ContentType = "application/json;charset=utf-8";
        }
    }
}
