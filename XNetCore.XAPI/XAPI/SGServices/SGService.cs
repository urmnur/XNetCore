using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using XNetCore.STL;
using XNetCore.XRun;

namespace XNetCore.XAPI
{
    /// <summary>
    /// SG服务类
    /// </summary>
    class SGService
    {
        public XResponse Excute(ApiRequest request, string param)
        {
            initializeHeaderData(request);
            var result = XRunner.Api(request.ServiceName, request.MethodName, param, XContext.Current.Token);
            return result;
        }
        /// <summary>
        /// 初始化haeader数据
        /// </summary>
        /// <param name="request"></param>
        private void initializeHeaderData(ApiRequest request)
        {
            try
            {
                XContext.Current.Token = request.GetVaule(XHeader.TOKEN);
                XContext.Current.TraceId = request.GetVaule(XHeader.TRACEID);
                XContext.Current.RpcId = request.GetVaule(XHeader.RPCID);
                XContext.Current.LogId = request.GetVaule(XHeader.LOGID).ToInt(0);
                XContext.Current.RequestId = request.GetVaule(XHeader.REQUESTID);
                XContext.Current.ClientIP = request.GetVaule(XHeader.CLIENTIP);
                if (string.IsNullOrWhiteSpace(XContext.Current.ClientIP))
                {
                    XContext.Current.ClientIP = request.IP;
                }
                XContext.Current.NewRpcId();
            }
            catch { }
        }

    }
}
