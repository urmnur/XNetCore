using System;
using XNetCore.RPC.Core;
using XNetCore.RPC.Greeter;
using Grpc.Core;
using XNetCore.STL;

namespace XNetCore.RPC.Server
{
    /// <summary>
    /// 服务执行
    /// </summary>
    class RpcParamHelper
    {
        #region 单例模式
        private static object lockobject = new object();
        private static RpcParamHelper _instance = null;
        public static RpcParamHelper Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (lockobject)
                    {
                        if (_instance == null)
                        {
                            _instance = new RpcParamHelper();
                        }
                    }

                }
                return _instance;
            }
        }
        private RpcParamHelper()
        {
        }
        #endregion


        public RpcCallContext ConvertParam(ServerCallContext context)
        {
            var rpcContext = new RpcCallContext(context.Method, context.Host, context.Peer, context);
            return rpcContext;
        }
        public ServiceRequest ConvertParam(RpcRequest rpc)
        {
            var request = rpc.Data.ToObject<ServiceRequest>();
            return request;
        }

        public Action<object> ConvertParam(RpcRequest rpc, IServerStreamWriter<RpcResponse> responseStream)
        {
            var request = ConvertParam(rpc);
            var service = RpcServiceStore.Instance.GetServiceRegistData(request.ServiceData);
            var result = new Action<object>(data =>
                    {
                        try
                        {
                            responseAction(service, request, data, responseStream);
                        }
                        catch { }
                    });
            return result;
        }


        private void responseAction(ServiceRegistData service, ServiceRequest request, object data, IServerStreamWriter<RpcResponse> responseStream)
        {
            if (responseStream == null)
            {
                return;
            }
            var d = ResponseHelper.Instance.GetResponse(service, request, data, null);
            var response = new RpcResponse();
            response.Data = d.ToJson();
            responseStream.WriteAsync(response).Wait();
        }


        public RpcResponse ConvertParam( ServiceResponse data)
        {
            var reponse = new RpcResponse();
            reponse.Data = data.ToJson();
            return reponse;
        }
    }
}
