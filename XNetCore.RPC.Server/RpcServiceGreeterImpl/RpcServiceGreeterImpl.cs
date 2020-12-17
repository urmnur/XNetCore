using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using XNetCore.RPC.Core;
using XNetCore.RPC.Greeter;

namespace XNetCore.RPC.Server
{
    class RpcServiceGreeterImpl : RpcServiceGreeter_Server.RpcServiceGreeterBase
    {
        public override Task<RpcResponse> RpcService(RpcRequest request, ServerCallContext context)
        {
            if (string.IsNullOrWhiteSpace(request.Data))
            {
                return Task.FromResult(new RpcResponse() { Data = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") });
            }
            var result = RpcServiceHelper.Instance.RpcService(RpcParamHelper.Instance.ConvertParam(context), RpcParamHelper.Instance.ConvertParam(request), null);
            var reponse = RpcParamHelper.Instance.ConvertParam(result);
            return Task.FromResult(reponse);
        }

        public override async Task RpcStream(RpcRequest request, IServerStreamWriter<RpcResponse> responseStream, ServerCallContext context)
        {
            if (string.IsNullOrWhiteSpace(request.Data))
            {
                return;
            }
            await RpcStreamAsync(request, responseStream, context);
        }
        private async Task RpcStreamAsync(RpcRequest request, IServerStreamWriter<RpcResponse> responseStream, ServerCallContext context)
        {
            while (true)
            {
                var result = RpcServiceHelper.Instance.RpcService(
                    RpcParamHelper.Instance.ConvertParam(context),
                    RpcParamHelper.Instance.ConvertParam(request),
                    RpcParamHelper.Instance.ConvertParam(request, responseStream));
                if (result.ServiceData.ServiceCode == -9999)
                {
                    break;
                }
                var reponse = RpcParamHelper.Instance.ConvertParam(result);
                await responseStream.WriteAsync(reponse);
                Thread.Sleep(2 * 1000);
            }
        }
    }
}
