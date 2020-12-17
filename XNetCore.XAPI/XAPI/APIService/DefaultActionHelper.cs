using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Linq;
using System.IO;
using XNetCore.STL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Http;

namespace XNetCore.XAPI
{
    class DefaultActionHelper
    {
        #region 单例模式
        private static object lockobject = new object();
        private static DefaultActionHelper _instance = null;
        public static DefaultActionHelper Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (lockobject)
                    {
                        if (_instance == null)
                        {
                            _instance = new DefaultActionHelper();
                        }
                    }

                }
                return _instance;
            }
        }
        private DefaultActionHelper()
        {
        }
        #endregion

        public object Run(Controller controller, string serviceName, string methodName)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var result = doSgServiceExcute(controller, serviceName, methodName);
            stopwatch.Stop();
            if (isHttpContent(result))
            {
                return result;
            }
            return responseResult(controller, result);
        }

        private bool isHttpContent(object value)
        {
            if (value == null)
            {
                return false;
            }
            return typeof(HttpResponseMessage).IsAssignableFrom(value.GetType());
        }

        private XResponse doSgServiceExcute(Controller controller, string serviceName, string methodName)
        {
            try
            {
                var result = SGServiceFactory.Instance.ServiceExcute(controller, serviceName, methodName);
                return result;
            }
            catch (Exception ex)
            {
                return new XResponse(ex);
            }
        }
        /// <summary>
        /// 返回数据格式化
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private object responseResult(Controller controller, XResponse value)
        {
            //controller.HttpContext.Response.ContentType = "application/json; charset=utf-8";
            return controller.HttpContext.Response.WriteAsync(value.ToJsResponse());
        }

    }
}
