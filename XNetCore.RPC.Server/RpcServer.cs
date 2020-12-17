using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using XNetCore.RPC.Core;
using XNetCore.RPC.Greeter;

namespace XNetCore.RPC.Server
{
    /// <summary>
    /// 服务运行类
    /// </summary>
    public class RpcServer
    {
        private Grpc.Core.Server server = null;
        public void Start(int port)
        {
            this.Start("0.0.0.0", port);
        }

        public void Start(string ip, int port)
        {
            if (server != null)
            {
                return;
            }
            CurrentAppRpcRegistData.Instance.Port = port;
            this.server = new Grpc.Core.Server(
                new List<ChannelOption>{
                    new ChannelOption(ChannelOptions.MaxReceiveMessageLength, int.MaxValue),
                    new ChannelOption(ChannelOptions.MaxSendMessageLength, int.MaxValue),
                })
            { 
                Services = {
                    RpcServiceGreeter_Server.BindService(
                    new RpcServiceGreeterImpl()
                    ),
                },
                Ports = { new ServerPort(ip, port, ServerCredentials.Insecure) }
            };
            this.server.Start();
        }

        public void Shutdown()
        {
            this.server.ShutdownAsync().Wait();
        }

        public static void Register(RegistData data)
        {
            if (data == null)
            {
                return;
            }
            RpcServiceStore.Instance.RegisterService(data);
        }
        public static void ClearRegister()
        {
            RpcServiceStore.Instance.Clear();
        }
    }

}