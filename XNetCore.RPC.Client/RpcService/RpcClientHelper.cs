using Castle.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XNetCore.RPC.Core;
using XNetCore.STL;

namespace XNetCore.RPC.Client
{
    class RpcClientHelper
    {
        #region 单例模式
        private static object lockobject = new object();
        private static RpcClientHelper _instance = null;
        public static RpcClientHelper Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (lockobject)
                    {
                        if (_instance == null)
                        {
                            _instance = new RpcClientHelper();
                        }
                    }

                }
                return _instance;
            }
        }
        private RpcClientHelper()
        {
        }
        #endregion

        private ProxyGenerator generator = new ProxyGenerator();

        private object getApi(Type type, IInterceptor[] interceptors)
        {
            var proxy = generator.CreateClassProxy(typeof(object), new Type[1]
               {
                 type
               },
               ProxyGenerationOptions.Default, new object[]
               {

               },
               interceptors);
            return proxy;
        }
        /// <summary>
        /// 获取Api操作类
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public object GetApi(Type type)
        {
            var interceptorList = new List<IInterceptor>();
            interceptorList.Add(new RpcInterceptor());
            return getApi(type, interceptorList.ToArray());
        }


        /// <summary>
        /// 获取Api操作类
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public object GetApi(Type type, string ip, int port)
        {
            List<IInterceptor> interceptorList = new List<IInterceptor>();
            interceptorList.Add(new RpcInterceptor(ip, port));
            return getApi(type, interceptorList.ToArray());
        }



        private ClientRegistData getRpcServiceDataByName(string serviceName)
        {
            var result = RpcServiceStore.Instance.GetServiceByName(serviceName);
            return result;
        }

        public ServiceResponse ExcuteService(string serviceName, string param)
        {
            var data = getRpcServiceDataByName(serviceName);
            var request = new ServiceRequest();
            request.ServiceData = new RequestServiceData();
            request.ServiceData.ServiceParam = param;
            var result = RpcService.Instance.ExcuteService(data, data?.ServerChannel, request);
            return result;
        }
    }
}
