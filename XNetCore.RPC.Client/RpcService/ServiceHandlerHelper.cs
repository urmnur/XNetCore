using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XNetCore.RPC.Core;
using XNetCore.STL;

namespace XNetCore.RPC.Client
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

        public ServiceRequest ClientRequest(ServiceRequest request)
        {
            //XNetCore.STL.LogClient.LogHelper.Instance.Debug("ClientRequest:" + request.ToJson());
            var handler = this.rpcServiceHandler;
            var registData = this.serviceRegistData.RegistData;
            if (handler == null)
            {
                return request;
            }
            request = handler.ClientRequest(registData, request);
            return request;
        }


        private void responseTrace(ServiceResponse data)
        {
            if (data == null || data.TraceData == null)
            {
                return;
            }
            XContext.Current.LogId = data.TraceData.LogId;
            //XContext.Current.TraceId = data.TraceData.TraceId;
            //XContext.Current.RpcId = data.TraceData.RpcId;
            XContext.Current.ClientIP = data.TraceData.ClientIP;
            XContext.Current.RequestId = data.TraceData.RequestId;
            var token = data.TraceData?.Token;
            XContext.Current.Token = token;
            XContext.Current.UserId = string.Empty;
            XContext.Current.IncreaseRpcId();
        }

        public ServiceResponse ClientRespons(ServiceRequest request, ServiceResponse response)
        {
            //XNetCore.STL.LogClient.LogHelper.Instance.Debug("ClientRespons:" + response.ToJson());
            responseTrace(response);
            var handler = this.rpcServiceHandler;
            var registData = this.serviceRegistData.RegistData;
            if (handler == null)
            {
                return response;
            }
            response = handler.ClientResponse(registData, request, response);
            return response;
        }
    }
}
