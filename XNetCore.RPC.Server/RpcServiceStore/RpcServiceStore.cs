using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XNetCore.RPC.Core;
using XNetCore.STL;

namespace XNetCore.RPC.Server
{
    public class RpcServiceStore
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
            servicestore = new ConcurrentDictionary<string, ServiceRegistData>();
        }

        private ConcurrentDictionary<string, ServiceRegistData> servicestore;

        public void RegisterService(RegistData data)
        {
            registerService(data);
        }

        public ServiceRegistData GetServiceRegistData(RequestServiceData data)
        {
            var result = getServiceRegistData(data);
            if (result == null)
            {
                result = getCurrentAppRpcServiceByIntfc(data);
            }

            if (result == null)
            {
                throw new XNetCore.STL.XExceptionNoFindInfc(data.ServiceInfc);
            }
            return result;
        }

        private ServiceRegistData getCurrentAppRpcServiceByIntfc(RequestServiceData data)
        {
            registerService(CurrentAppRpcRegistData.Instance.CreateRegistData(data.ServiceInfc, data.ServiceMethod));
            return getServiceRegistData(data);
        }

        private void registerService(RegistData data)
        {
            if (data == null || string.IsNullOrWhiteSpace(data.ServiceInfc)
                || string.IsNullOrWhiteSpace(data.ServiceImpl))
            {
                return;
            }
            var value = new ServiceRegistData(data);
            servicestore.TryAdd(Guid.NewGuid().ToString(), value);
        }



        private bool nameCompare(string name1, string name2)
        {
            if (string.IsNullOrWhiteSpace(name1) || string.IsNullOrWhiteSpace(name2))
            {
                return false;
            }
            var t1 = name1.ToLower().Replace(" ", string.Empty);
            var t2 = name2.ToLower().Replace(" ", string.Empty);
            return t1 == t2;
        }

        private ServiceRegistData getServiceRegistDataByInfc(string infc)
        {
            var values = this.servicestore.Values.ToList();
            foreach (var value in values)
            {
                var server = value.RegistData;
                if (nameCompare(server.ServiceInfc, infc))
                {
                    return value;
                }
            }
            return null;
        }
        private ServiceRegistData getServiceRegistDataByMethod(string infc, string method)
        {
            var values = this.servicestore.Values.ToList();
            foreach (var value in values)
            {
                var server = value.RegistData;
                if (nameCompare(server.ServiceInfc, infc)
                    && nameCompare(server.ServiceMethod, method))
                {
                    return value;
                }
            }
            return null;
        }



        private ServiceRegistData getServiceRegistDataById(string id)
        {
            var values = this.servicestore.Values.ToList();
            foreach (var value in values)
            {
                var server = value.RegistData;
                if (server.ServiceId == id)
                {
                    return value;
                }
            }
            return null;
        }

        private ServiceRegistData getServiceRegistDataByName(string name)
        {
            var values = this.servicestore.Values.ToList();
            foreach (var value in values)
            {
                var server = value.RegistData;
                if (server.ServiceName == name)
                {
                    return value;
                }
            }
            return null;
        }

        private ServiceRegistData getServiceRegistData(RequestServiceData data)
        {
            ServiceRegistData result = null;
            var id = data.ServiceId;
            if (!string.IsNullOrWhiteSpace(id))
            {
                result = getServiceRegistDataById(id);
            }
            if (result != null)
            {
                return result;
            }
            var name = data.ServiceName;
            if (!string.IsNullOrWhiteSpace(name))
            {
                result = getServiceRegistDataByName(name);
            }
            if (result != null)
            {
                return result;
            }
            var infc = data.ServiceInfc;
            var method = data.ServiceMethod;
            if (!string.IsNullOrWhiteSpace(infc) && !string.IsNullOrWhiteSpace(method))
            {
                result = getServiceRegistDataByMethod(infc, method);
            }
            if (result != null)
            {
                return result;
            }
            result = getServiceRegistDataByInfc(infc);
            return result;
        }
    }

}
