using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XNetCore.RPC.Core;
using XNetCore.STL;

namespace XNetCore.RPC.Server
{
    public class ResponseHelper
    {
        #region 单例模式
        private static object lockobject = new object();
        private static ResponseHelper _instance = null;
        public static ResponseHelper Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (lockobject)
                    {
                        if (_instance == null)
                        {
                            _instance = new ResponseHelper();
                        }
                    }

                }
                return _instance;
            }
        }
        private ResponseHelper()
        {
        }
        #endregion

        public ServiceResponse GetResponse(ServiceRegistData service, ServiceRequest request, object data, Exception ex)
        {
            if (ex == null)
            {
                return successServiceResponse(service, request, data);
            }
            else
            {
                return errServiceResponse(service, request, ex);
            }

        }
        private void sendMsg(string msg)
        {
        }
        private void sendErrMsg(string msg)
        {
        }
        private XException getXException(Exception ex)
        {
            var result = ex as XException;
            if (result != null)
            {
                return result;
            }
            if (ex.InnerException == null)
            {
                return result;
            }
            result = ex.InnerException as XException;
            if (result != null)
            {
                return result;
            }
            return result;
        }
        private ServiceResponse successServiceResponse(ServiceRegistData service, ServiceRequest request, object data)
        {
            sendMsg($"RPC Success>>[{request.TraceData.RequestId}.{request.TraceData.TraceId}]>>[{request.ServiceData.ServiceInfc}].[{request.ServiceData.ServiceMethod}]");
            var result = getServiceResponse(service, null);
            if (data != null)
            {
                result.ServiceData.ServiceData = data.ToJson();
                result.ServiceData.ServiceDataType = data.GetType().AssemblyQualifiedName;
            }
            return result;
        }
        private ServiceResponse errServiceResponse(ServiceRegistData service, ServiceRequest request, Exception ex)
        {
            var xex = getXException(ex);
            if (xex == null)
            {
                sendErrMsg($"RPC  Failed>>[{request.TraceData.RequestId}.{request.TraceData.TraceId}]>>[{request.ServiceData.ServiceInfc}].[{request.ServiceData.ServiceMethod}]\r\n{ex.ToString()}");
                return FaillResponse(service, ex);
            }
            else
            {
                sendErrMsg($"RPC  Failed>>[{request.TraceData.RequestId}.{request.TraceData.TraceId}]>>[{request.ServiceData.ServiceInfc}].[{request.ServiceData.ServiceMethod}]\r\n{xex.ToString()}");
                return FaillResponse(service, xex);
            }
        }

        private ServiceResponse FaillResponse(ServiceRegistData service, Exception ex)
        {
            var result = getServiceResponse(service, null);
            result.ServiceData.ServiceCode = 99990;
            result.ServiceData.ServiceMsg = string.Empty;
            if (ex != null)
            {
                result.ServiceData.ServiceError = ex.ToString();
                if (ex is XException)
                {
                    var h = ex as XException;
                    result.ServiceData.ServiceCode = h.ErrCode;
                    result.ServiceData.ServiceMsg = string.Empty + h.ErrMsg;
                    if (h.ErrData != null)
                    {
                        result.ServiceData.ServiceData = h.ErrData.ToJson();
                        result.ServiceData.ServiceDataType = h.ErrData.GetType().FullName();
                    }
                }
            }
            return result;
        }
        private ServiceResponse getServiceResponse(ServiceRegistData service, ServiceResponse response)
        {
            if (response == null)
            {
                response = new ServiceResponse();
            }
            response = getResponseServiceData(service, response);
            response = getResponseTraceData(service, response);
            return response;
        }

        private ServiceResponse getResponseServiceData(ServiceRegistData service, ServiceResponse response)
        {
            if (response.ServiceData == null)
            {
                response.ServiceData = new ResponseServiceData();
            }
            if (response.ServiceMetadata == null)
            {
                response.ServiceMetadata = new ResponseServiceMetadata();
            }
            if (service != null)
            {
                response.ServiceMetadata.ServiceId = service.RegistData.ServiceId;
                response.ServiceMetadata.ServiceProvderId = service.RegistData.ServiceProvderId;
                response.ServiceMetadata.ServiceName = service.RegistData.ServiceName;
                response.ServiceMetadata.ServiceInfc = service.RegistData.ServiceInfc;
                response.ServiceMetadata.ServiceImpl = service.RegistData.ServiceImpl;
                response.ServiceMetadata.ServiceMethod = service.RegistData.ServiceMethod;
                response.ServiceMetadata.ServiceHandler = service.RegistData.ServiceHandler;
                response.ServiceMetadata.Model = service.RegistData.Model;
                response.ServiceMetadata.Auth = service.RegistData.Auth;
            }
            response.ServiceData.ServiceCode = 0;
            response.ServiceData.ServiceMsg = string.Empty;
            response.ServiceData.ServiceError = string.Empty;
            response.ServiceData.ServiceData = string.Empty;
            response.ServiceData.ServiceDataType = string.Empty;

            return response;
        }



        private ServiceResponse getResponseTraceData(ServiceRegistData service, ServiceResponse response)
        {
            if (response.TraceData == null)
            {
                response.TraceData = new ResponseTraceData();
            }
            response.TraceData.RequestId = XContext.Current.RequestId;
            response.TraceData.TraceId = XContext.Current.TraceId;
            response.TraceData.RpcId = XContext.Current.RpcId;
            response.TraceData.ClientIP = XContext.Current.ClientIP;
            response.TraceData.Token = XContext.Current.Token;
            response.TraceData.LogId = XContext.Current.LogId;
            return response;
        }


    }
}
