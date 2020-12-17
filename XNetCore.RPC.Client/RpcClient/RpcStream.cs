using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using XNetCore.RPC.Core;
using XNetCore.RPC.Greeter;
using XNetCore.STL;

namespace XNetCore.RPC.Client
{
    /// <summary>
    /// 远程服务
    /// </summary>
    class RpcStream
    {
        #region 单例模式
        private static object lockobject = new object();
        private static RpcStream _instance = null;
        public static RpcStream Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (lockobject)
                    {
                        if (_instance == null)
                        {
                            _instance = new RpcStream();
                        }
                    }

                }
                return _instance;
            }
        }
        private RpcStream()
        {
        }
        #endregion

        /// <summary>
        /// 远程服务调用
        /// </summary>
        /// <param name="service"></param>
        /// <param name="request"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public void ExcuteService(ClientRegistData service, Channel channel, ServiceRequest request, Delegate action)
        {
            Task.Run(() =>
            {
                try
                {
                    doExcuteService(service, channel, request, action).Wait();
                }
                catch (Exception ex)
                {

                }
            });
        }

        private async Task doExcuteService(ClientRegistData service, Channel channel, ServiceRequest request, Delegate action)
        {
            var handler = new ServiceHandlerHelper(service);
            var rpcChannel = channel;
            if (rpcChannel == null)
            {
                rpcChannel = service.ServerChannel;
            }
            var client = new RpcServiceGreeter_Client.RpcServiceGreeterClient(rpcChannel);
            request = new RequestHelper().GetServiceRequest(service, request);
            request = handler.ClientRequest(request);
            await doExcuteService(request, action, client);
        }


        private async Task doExcuteService(ServiceRequest request, Delegate action, RpcServiceGreeter_Client.RpcServiceGreeterClient client)
        {
            var options = new CallOptions();
            request.TraceData.LogId = XContext.Current.LogId;
            var param = new RpcRequest();
            param.Data = string.Empty + request.ToJson();
            var responses = client.RpcStream(param, options);
            while (await responses.ResponseStream.MoveNext())
            {
                try
                {
                    var response = responses.ResponseStream.Current;
                    if (response == null)
                    {
                        continue;
                    }
                    if (string.IsNullOrWhiteSpace(response.Data))
                    {
                        continue;
                    }
                    var result = response.Data.ToObject<ServiceResponse>();

                    runMethod(action, request, result);
                }
                catch { }
            }
        }

        private void runMethod(Delegate action, ServiceRequest request, ServiceResponse response)
        {
            var pType = action.GetMethodInfo().GetParameters()[0].ParameterType;
            var obj = getMethodParam(pType, request, response);
            action.DynamicInvoke(obj);
        }
        private object getMethodParam(Type pType, ServiceRequest request, ServiceResponse response)
        {
            if (pType.IsAssignableFrom(typeof(ServiceRequest)))
            {
                return request;
            }
            if (pType.IsAssignableFrom(typeof(ServiceResponse)))
            {
                return response;
            }
            var paramJson = response.ServiceData.ServiceData;
            return paramJson.ToObject(pType);
        }
    }
}
