using Castle.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XNetCore.XAPI.Client;
using XNetCore.STL;

namespace XNetCore.XAPI.Client
{
    class XApiClientHelper
    {
        #region 单例模式
        private static object lockobject = new object();
        private static XApiClientHelper _instance = null;
        public static XApiClientHelper Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (lockobject)
                    {
                        if (_instance == null)
                        {
                            _instance = new XApiClientHelper();
                        }
                    }

                }
                return _instance;
            }
        }
        private XApiClientHelper()
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
            interceptorList.Add(new XApiInterceptor());
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
            interceptorList.Add(new XApiInterceptor(ip, port));
            return getApi(type, interceptorList.ToArray());
        }



    }
}
