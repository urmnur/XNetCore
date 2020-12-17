using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XNetCore.RPC.Core
{
    public interface RpcServiceHandler
    {
        ServiceRequest ClientRequest(RegistData client, ServiceRequest request);
        ServiceRequest ServerRequest(RegistData server, ServiceRequest request);
        ServiceResponse ClientResponse(RegistData client, ServiceRequest request, ServiceResponse response);
        ServiceResponse ServerResponse(RegistData server, ServiceRequest request, ServiceResponse response);
        string[] ContextParamKeys { get; }
        string GetHandlerParamValue(string key);
    }

    public class RequestServiceData
    {
        public string ServiceId { get; set; }
        public string ServiceProvderId { get; set; }
        public string ServiceName { get; set; }
        public string ServiceInfc { get; set; }
        public string ServiceImpl { get; set; }
        public string ServiceMethod { get; set; }
        public string ServiceParam { get; set; }
    }

    public class ResponseServiceMetadata
    {
        public string ServiceId { get; set; }
        public string ServiceProvderId { get; set; }
        public string ServiceName { get; set; }
        public string ServiceInfc { get; set; }
        public string ServiceImpl { get; set; }
        public string ServiceMethod { get; set; }
        public string ServiceHandler { get; set; }
        public int Model { get; set; }
        public int Auth { get; set; }
    }
    public class ResponseServiceData
    {
        public int ServiceCode { get; set; }
        public string ServiceMsg { get; set; }
        public string ServiceError { get; set; }
        public string ServiceData { get; set; }
        public string ServiceDataType { get; set; }
    }

    public class RequestTraceData
    {
        public string AppClusterId { get; set; }
        public string RequestId { get; set; }
        public string TraceId { get; set; }
        public string RpcId { get; set; }
        public string ClientIP { get; set; }
        public string Token { get; set; }
        public int LogId { get; set; }
    }

    public class ResponseTraceData
    {
        public string RequestId { get; set; }
        public string TraceId { get; set; }
        public string RpcId { get; set; }
        public string ClientIP { get; set; }
        public string Token { get; set; }
        public int LogId { get; set; }
    }

    public class ServiceRequest
    {
        public ServiceRequest()
        {
            this.RequestTime = DateTime.Now;
        }
        public DateTime RequestTime { get; set; }
        public string HandlerExtendData { get; set; }
        public RequestServiceData ServiceData { get; set; }

        public RequestTraceData TraceData { get; set; }
    }

    public class ServiceResponse
    {
        public ServiceResponse()
        {
            this.ResponseTime = DateTime.Now;
        }
        public DateTime ResponseTime { get; set; }
        public string HandlerExtendData { get; set; }
        public ResponseServiceData ServiceData { get; set; }
        public ResponseServiceMetadata ServiceMetadata { get; set; }
        public ResponseTraceData TraceData { get; set; }
    }
}
