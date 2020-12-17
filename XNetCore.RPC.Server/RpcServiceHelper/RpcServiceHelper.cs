using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using XNetCore.RPC.Core;
using XNetCore.STL;

namespace XNetCore.RPC.Server
{
    /// <summary>
    /// 服务执行
    /// </summary>
    public class RpcServiceHelper
    {
        #region 单例模式
        private static object lockobject = new object();
        private static RpcServiceHelper _instance = null;
        public static RpcServiceHelper Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (lockobject)
                    {
                        if (_instance == null)
                        {
                            _instance = new RpcServiceHelper();
                        }
                    }

                }
                return _instance;
            }
        }
        private RpcServiceHelper()
        {
        }
        #endregion


        public ServiceResponse RpcService(RpcCallContext context, ServiceRequest request, Action<object> responseAction)
        {
            requestTrace(request);
            var service = RpcServiceStore.Instance.GetServiceRegistData(request.ServiceData);
            var result = doRsfService(service, request, responseAction, context);
            return result;
        }
        private void sendMsg(RpcCallContext context, string msg)
        {
        }
        private void requestTrace(ServiceRequest data)
        {
            if (data?.TraceData == null)
            {
                XContext.Current.NewTraceId();
                return;
            }
            XContext.Current.LogId = data.TraceData.LogId;
            XContext.Current.TraceId = data.TraceData.TraceId;
            XContext.Current.RpcId = data.TraceData.RpcId;
            XContext.Current.ClientIP = data.TraceData.ClientIP;
            XContext.Current.RequestId = data.TraceData.RequestId;
            var token = data.TraceData?.Token;
            XContext.Current.Token = token;
            XContext.Current.UserId = string.Empty;
            XContext.Current.IncreaseRpcId();
        }


        private ServiceResponse doRsfService(ServiceRegistData service, ServiceRequest request, Action<object> responseAction, RpcCallContext context)
        {
            try
            {
                var handler = new ServiceHandlerHelper(service);
                var result = doRsfService(service, request, responseAction, context, handler);
                return handler.ServerResponse(request, result);
            }
            catch (Exception ex)
            {
                return ResponseHelper.Instance.GetResponse(service, request, null, ex);
            }
        }
        private ServiceResponse doRsfService(ServiceRegistData service, ServiceRequest request, Action<object> responseAction, RpcCallContext context, ServiceHandlerHelper handler)
        {
            try
            {
                sendMsg(context, $"RPC   Start>>[{request.TraceData.RequestId}.{request.TraceData.TraceId}]>>[{request.ServiceData.ServiceInfc}].[{request.ServiceData.ServiceMethod}]");
                request = handler.ServerRequest(request);
                var result = doExcuteService(service, request, responseAction, context);
                return ResponseHelper.Instance.GetResponse(service, request, result, null);
            }
            catch (Exception ex)
            {
                return ResponseHelper.Instance.GetResponse(service, request, null, ex);
            }
        }
        /// <summary>
        /// 服务执行
        /// </summary>
        /// <param name="metadata"></param>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        private object doExcuteService(ServiceRegistData service, ServiceRequest request, Action<object> responseAction, RpcCallContext context)
        {
            var method = new RpcMethodExcute();
            var result = method.Excute(service, request, responseAction, context);
            return result;
        }


    }
}