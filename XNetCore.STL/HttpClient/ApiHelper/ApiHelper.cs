using Castle.DynamicProxy;
using System;
using System.Collections.Generic;

namespace XNetCore.STL
{
    class ApiHelper
    {
        #region 单例模式
        private static object lockobject = new object();
        private static ApiHelper _instance = null;
        public static ApiHelper Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (lockobject)
                    {
                        if (_instance == null)
                        {
                            _instance = new ApiHelper();
                        }
                    }

                }
                return _instance;
            }
        }
        private ApiHelper()
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
        public object GetApi(Type type,string url)
        {
            var interceptorList = new List<IInterceptor>();
            interceptorList.Add(new ApiInterceptor(url));
            return getApi(type, interceptorList.ToArray());
        }
    }
}
