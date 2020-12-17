using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XNetCore.STL;

namespace XNetCore.RPC.Core
{
    public class RpcCallContext
    {
        public RpcCallContext(string method,string host,string peer,object serverCallContext)
        {
            this.Method = method;
            this.Host = host;
            this.Peer = peer;
            this.PeerAddress = new PeerAddress(this);
            this.ServerCallContext = serverCallContext;
        }
        public string Method { get; private set; }
        public string Host { get; private set; }
        public string Peer { get; private set; }
        public PeerAddress PeerAddress { get; private set; }
        public object ServerCallContext { get; private set; }
    }


    /// <summary>
    /// 客户端地址
    /// </summary>
    public class PeerAddress
    {
        public PeerAddress(RpcCallContext context)
        {
            iniPeerAddress(context);
        }
        
        /// <summary>
        /// 解析客户端地址
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
       private void iniPeerAddress(RpcCallContext context)
        {
            
            var peer = context.Peer;
            var idx1 = peer.IndexOf(':');
            if (idx1 > 0)
            {
                this.IpVersion = peer.Substring(0, idx1);
            }
            var idx2 = peer.LastIndexOf(':');
            if (idx2 > idx1)
            {
                this.IpAddress = peer.Substring(idx1 + 1, idx2 - idx1 - 1);
                if (this.IpAddress == "[::1]")
                {
                    this.IpAddress = "127.0.0.1";
                }
                this.Port = peer.Substring(idx2 + 1).ToInt(0);
            }
        }
        /// <summary>
        /// IP版本
        /// </summary>
        public string IpVersion { get;private set; }
        /// <summary>
        /// IP地址
        /// </summary>
        public string IpAddress { get; private set; }
        /// <summary>
        /// 端口号
        /// </summary>
        public int Port { get; private set; }
    }
}
