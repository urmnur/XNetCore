using Grpc.Core;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XNetCore.RPC.Core;
using XNetCore.STL;

namespace XNetCore.RPC.Client
{
    class RpcServiceStore
    {
        #region 单例模式
        private static object lockobject = new object();
        private static RpcServiceStore _instance = null;
        public static RpcServiceStore Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (lockobject)
                    {
                        if (_instance == null)
                        {
                            _instance = new RpcServiceStore();
                        }
                    }

                }
                return _instance;
            }
        }
        private RpcServiceStore()
        {
            Clear();
        }
        #endregion
        public void Clear()
        {
            servicestore = new ConcurrentDictionary<string, ClientRegistData>();
        }
        private ConcurrentDictionary<string, ClientRegistData> servicestore;

        public void RegisterService(RegistData data)
        {
            if (data == null || string.IsNullOrWhiteSpace(data.ServiceInfc))
            {
                return;
            }
            if (this.GetServiceByIntfc(data.ServiceInfc, data.ServiceMethod) != null)
            {
                return;
            }
            var value = new ClientRegistData(data);
            servicestore.TryAdd(Guid.NewGuid().ToString(), value);
        }

        private bool RpcImplEnable(RegistData data)
        {
            if (data == null)
            {
                return false;
            }
            if (string.IsNullOrWhiteSpace(data.ServiceImpl))
            {
                return false;
            }
            return Type.GetType(data?.ServiceImpl, false, true) != null;
        }


        private bool typeNameCompare(string typeName1, string typeName2)
        {
            if (string.IsNullOrWhiteSpace(typeName1) || string.IsNullOrWhiteSpace(typeName2))
            {
                return false;
            }
            var t1 = typeName1.ToLower().Replace(" ", string.Empty);
            var t2 = typeName2.ToLower().Replace(" ", string.Empty);
            return t1 == t2;
        }

        public ClientRegistData GetServiceByIntfc(string serviceIntfc, string methodName)
        {
            var result = getServiceByIntfc(serviceIntfc, methodName);
            if (result == null)
            {
                result = getCurrentAppRpcServiceByIntfc(serviceIntfc, methodName);
            }
            return result;
        }
        private ClientRegistData getServiceByIntfc(string serviceIntfc, string methodName)
        {
            var result1 = new List<ClientRegistData>();
            var result2 = new List<ClientRegistData>();
            var values = this.servicestore.Values;
            foreach (var info in values)
            {
                if (typeNameCompare(info.RegistData.ServiceInfc, serviceIntfc))
                {
                    result1.Add(info);
                    if (string.IsNullOrWhiteSpace(info.RegistData.ServiceMethod)
                        || string.IsNullOrWhiteSpace(methodName))
                    {
                        continue;
                    }
                    if (info.RegistData.ServiceMethod.ToLower().Trim() == methodName.ToLower().Trim())
                    {
                        result2.Add(info);
                    }
                }
            }
            if (result1.Count == 0)
            {
                return null;
            }
            if (result2.Count > 0)
            {
                return getRandomInstance(result2.ToArray());
            }
            return getRandomInstance(result1.ToArray());
        }

        private ClientRegistData getCurrentAppRpcServiceByIntfc(string serviceIntfc, string methodName)
        {
            var data = CurrentAppRpcRegistData.Instance.CreateRegistData(serviceIntfc, methodName);
            if (data == null)
            {
                return null;
            }
            RegisterService(data);
            return getServiceByIntfc(serviceIntfc, methodName);
        }



        public ClientRegistData GetServiceByName(string serviceName)
        {
            var result = new List<ClientRegistData>();
            var values = this.servicestore.Values;
            foreach (var info in values)
            {
                if (typeNameCompare(info.RegistData.ServiceName, serviceName))
                {
                    result.Add(info);
                }
            }
            if (result.Count == 0)
            {
                throw new XNetCore.STL.XExceptionNoFindService(serviceName);
            }
            return getRandomInstance(result.ToArray());
        }



        /// <summary>
        /// 获取远程服务通道
        /// </summary>
        /// <param name="channels"></param>
        /// <returns></returns>
        private T getRandomInstance<T>(T[] channels)
        {
            if (channels.Length == 0)
            {
                return default(T);
            }
            var random = new Random(Guid.NewGuid().GetHashCode());
            var idx = random.Next(0, channels.Length);
            var result = channels[idx];
            return result;
        }
    }


}
