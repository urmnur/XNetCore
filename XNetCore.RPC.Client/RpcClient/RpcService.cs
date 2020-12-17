using Grpc.Core;
using XNetCore.RPC.Core;
using XNetCore.RPC.Greeter;
using XNetCore.STL;

namespace XNetCore.RPC.Client
{
    /// <summary>
    /// 远程服务
    /// </summary>
    class RpcService
    {
        #region 单例模式
        private static object lockobject = new object();
        private static RpcService _instance = null;
        public static RpcService Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (lockobject)
                    {
                        if (_instance == null)
                        {
                            _instance = new RpcService();
                        }
                    }

                }
                return _instance;
            }
        }
        private RpcService()
        {
        }
        #endregion

        public ServiceResponse ExcuteService(ClientRegistData service, Channel channel, ServiceRequest request)
        {
            return doExcuteService(service, channel, request);
        }

        /// <summary>
        /// 远程服务调用
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        private ServiceResponse doExcuteService(ClientRegistData service, Channel channel, ServiceRequest request)
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
            var result = excuteService(service, request, client);
            result = handler.ClientRespons(request, result);
            return result;
        }



        private ServiceResponse excuteService(ClientRegistData service, ServiceRequest request, RpcServiceGreeter_Client.RpcServiceGreeterClient client)
        {
            return rpcExcuteService(request, client);
        }


        /// <summary>
        /// 远程服务调用
        /// </summary>
        /// <param name="request"></param>
        /// <param name="client"></param>
        /// <returns></returns>
        private ServiceResponse rpcExcuteService(ServiceRequest request, RpcServiceGreeter_Client.RpcServiceGreeterClient client)
        {
            var options = new CallOptions();
            request.TraceData.LogId = XContext.Current.LogId;
            var param = new RpcRequest();
            param.Data = string.Empty + request.ToJson();
            var response = client.RpcService(param, options);
            var result = response.Data.ToObject<ServiceResponse>();
            return result;
        }
    }

}
