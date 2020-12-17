using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XNetCore.RPC.Core;
using XNetCore.STL;

namespace XNetCore.RPC.Client
{
    class RequestHelper
    {
        public ServiceRequest GetServiceRequest(ServiceRegistData service, ServiceRequest request)
        {
            if (request == null)
            {
                request = new ServiceRequest();
            }
            request = getRequestServiceData(service, request);
            request = getRequestTraceData(service, request);
            return request;
        }


        private ServiceRequest getRequestServiceData(ServiceRegistData service, ServiceRequest request)
        {
            if (request.ServiceData == null)
            {
                request.ServiceData = new RequestServiceData();
            }
            request.ServiceData.ServiceId = service.RegistData.ServiceId;
            request.ServiceData.ServiceProvderId = service.RegistData.ServiceProvderId;
            request.ServiceData.ServiceName = service.RegistData.ServiceName;
            request.ServiceData.ServiceInfc = service.RegistData.ServiceInfc;
            request.ServiceData.ServiceImpl = service.RegistData.ServiceImpl;
            if (string.IsNullOrWhiteSpace(request.ServiceData.ServiceMethod))
            {
                request.ServiceData.ServiceMethod = service.RegistData.ServiceMethod;
            }
            return request;
        }



        private ServiceRequest getRequestTraceData(ServiceRegistData service, ServiceRequest request)
        {
            if (request.TraceData == null)
            {
                request.TraceData = new RequestTraceData();
            }
            request.TraceData.AppClusterId = XApp.Current.AppId;
            request.TraceData.RequestId = Guid.NewGuid().ToString();
            request.TraceData.TraceId = XContext.Current.TraceId;
            request.TraceData.RpcId = XContext.Current.RpcId;
            request.TraceData.ClientIP = XContext.Current.ClientIP;
            request.TraceData.Token = XContext.Current.Token;
            request.TraceData.LogId = XContext.Current.LogId;
            return request;
        }
    }
}
