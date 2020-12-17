using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using XNetCore.RPC.Core;
using XNetCore.STL;
using XNetCore.XRun;

namespace XNetCore.RPC.Server
{
    /// <summary>
    /// SPI服务执行
    /// </summary>
    class RpcMethodExcute
    {
        /// <summary>
        /// 服务执行
        /// </summary>
        /// <param name="metadata"></param>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public object Excute(ServiceRegistData service, ServiceRequest request, Action<object> responseAction, RpcCallContext context)
        {
            var result = doExcute(service, request, responseAction, context);
            return result;
        }
        
        /// <summary>
        /// 服务执行
        /// </summary>
        /// <param name="metadata"></param>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        private object doExcute(ServiceRegistData service, ServiceRequest request, Action< object> responseAction, RpcCallContext context)
        {
            var typeParams = new List<object>();
            typeParams.Add(service);
            typeParams.Add(request);
            typeParams.Add(context);
            if (context?.ServerCallContext != null)
            {
                typeParams.Add(context.ServerCallContext);
            }


            var paramValue = context?.PeerAddress?.IpAddress;

            var nameParams = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            if (!string.IsNullOrWhiteSpace(paramValue))
            {
                nameParams.Add("ip", paramValue);
            }
            else
            {
                nameParams.Add("ip", "127.0.0.1");
            }
            var result = XRunner.Invoke(service.ServiceImpl, request.ServiceData.ServiceMethod, request.ServiceData.ServiceParam, nameParams, typeParams, responseAction);
            return result;
        }
    }
}
