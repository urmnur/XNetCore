using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XNetCore.RPC.Core;
using XNetCore.STL;

namespace XNetCore.RPC.Server
{
    class ServiceHandlerHelper
    {
        public ServiceHandlerHelper(ServiceRegistData data)
        {
            this.serviceRegistData = data;
            var type = data.ServiceHandler;
            if (type == null)
            {
                return;
            }
            this.rpcServiceHandler = Activator.CreateInstance(type, null) as RpcServiceHandler;
        }



        private ServiceRegistData serviceRegistData;
        private RpcServiceHandler rpcServiceHandler;



        public ServiceRequest ServerRequest(ServiceRequest request)
        {
            //XNetCore.STL.LogClient.LogHelper.Instance.Debug("ServerRequest:" + request.ToJson());
            var handler = this.rpcServiceHandler;
            var registData = this.serviceRegistData.RegistData;
            if (handler == null)
            {
                return request;
            }
            request = handler.ServerRequest(registData, request);
            return request;
        }


        public ServiceResponse ServerResponse(ServiceRequest request, ServiceResponse response)
        {
            //XNetCore.STL.LogClient.LogHelper.Instance.Debug("ServerResponse:" + response.ToJson());
            var handler = this.rpcServiceHandler;
            var registData = this.serviceRegistData.RegistData;
            if (handler == null)
            {
                return response;
            }
            response = handler.ServerResponse(registData, request, response);
            response.TraceData.LogId = STL.XContext.Current.LogId;
            return response;
        }

        public bool IsHandlerParamKey(string key)
        {
            var handler = this.rpcServiceHandler;
            if (handler == null || handler.ContextParamKeys == null)
            {
                return false;
            }
            foreach (var n in handler.ContextParamKeys)
            {
                if (string.IsNullOrWhiteSpace(n))
                {
                    continue;
                }
                if (n.ToLower() == key.ToLower())
                {
                    return true;
                }
            }
            return false;
        }


        public string GetHandlerParamValue(string key)
        {
            var handler = this.rpcServiceHandler;
            if (handler == null)
            {
                return string.Empty;
            }
            return handler.GetHandlerParamValue(key);
        }

    }
}
