using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XNetCore.RPC.Core;

namespace XNetCore.RPC.Client
{
    class ClientRegistData :  ServiceRegistData
    {
        public ClientRegistData(RegistData data) : base(data)
        {
            this.ServerChannel = this.CreateChannel(data.Address, data.Port);
        }
        public Channel ServerChannel { get; private set; }

        public Channel CreateChannel(string ip, int port)
        {
            if (string.IsNullOrWhiteSpace(ip))
            {
                return null;
            }
            return new Channel(ip, port, ChannelCredentials.Insecure,
                 new List<ChannelOption>{
                    new ChannelOption(ChannelOptions.MaxReceiveMessageLength, int.MaxValue),
                    new ChannelOption(ChannelOptions.MaxSendMessageLength, int.MaxValue),
                 });
        }
        
    }
}
