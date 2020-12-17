using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web;
using Microsoft.AspNetCore.Mvc;
using XNetCore.STL;

namespace XNetCore.XAPI
{
    /// <summary>
    /// SG服务类
    /// </summary>
    class SGServiceFactory
    {
        #region 单例模式
        private static object lockobject = new object();
        private static SGServiceFactory _instance = null;
        public static SGServiceFactory Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (lockobject)
                    {
                        if (_instance == null)
                        {
                            _instance = new SGServiceFactory();
                        }
                    }

                }
                return _instance;
            }
        }
        private SGServiceFactory()
        {
        }
        #endregion

        public XResponse ServiceExcute(Controller controller, string serviceName, string methodName)
        {
            var request = new ApiRequest(controller, serviceName, methodName);
            var result = new SGService().Excute(request, request.Body);
            return result;
        }

    }
}
