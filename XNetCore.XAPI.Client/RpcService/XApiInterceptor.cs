using Castle.DynamicProxy;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using XNetCore.XAPI.Client;
using XNetCore.STL;

namespace XNetCore.XAPI.Client
{
    /// <summary>
    /// XApi操作拦截器
    /// </summary>
    class XApiInterceptor : IInterceptor
    {

        public XApiInterceptor()
        {
        }

        private string XApiIp = string.Empty;
        private int XApiPort = 0;

        public XApiInterceptor(string ip, int port)
        {
            this.XApiIp = ip;
            this.XApiPort = port;
        }

        /// <summary>
        /// 拦截器执行
        /// </summary>
        /// <param name="invocation"></param>
        public void Intercept(IInvocation invocation)
        {
            InterceptExcute(invocation);
        }

        private ClientRegistData getXApiServiceDataByIntfc(MethodInfo method)
        {
            var typeName = method.DeclaringType.FullName();
            var result = XApiServiceStore.Instance.GetServiceByIntfc(typeName, method.Name);
            if (result == null)
            {
                throw new XNetCore.STL.XExceptionNoFindInfc($"{typeName}.{method.Name}");
            }
            return result;
        }
        private void InterceptExcute(IInvocation invocation)
        {
            //DeclaringType：声明方法的类；ReflectedType：当前的反射类
            var method = invocation.Method;
            if (method == null || method.DeclaringType == (Type)null)
            {
                return;
            }
            var ps = method.GetParameters();
            var param = new Dictionary<string, object>();
            for (int i = 0; i < ps.Length; i++)
            {
                var value = invocation.Arguments[i];
                param.Add(ps[i].Name, value);
            }
            var servicedata = getXApiServiceDataByIntfc(method);
            var methodName = method.Name;
            if (!string.IsNullOrWhiteSpace(servicedata?.RegistData?.ServiceMethod))
            {
                var serviceMethod = servicedata.RegistData.ServiceMethod;
                if (serviceMethod.ToLower().EndsWith("."+ methodName.ToLower()))
                {
                    methodName = serviceMethod;
                }
            }
            var response = XApiService.Instance.ExcuteService(servicedata, methodName, param.ToJson());
            invocation.ReturnValue = getXApiResult(response, method.ReturnType);
        }



        /// <summary>
        /// 获取Api结果
        /// </summary>
        /// <param name="response"></param>
        /// <param name="resultType"></param>
        /// <returns></returns>
        private object getXApiResult(XResponse response, Type resultType)
        {
            if (response == null)
            {
                return null;
            }
            if (!string.IsNullOrWhiteSpace(response.Error))
            {
                var msg = response.Error;
                if (!string.IsNullOrWhiteSpace(response.Msg))
                {
                    msg = response.Msg;
                }
                throw new XNetCore.STL.XException(msg)
                {
                    ErrCode = response.Code,
                    ErrData = response.Data,
                    ErrMsg = msg,
                };
            }
            var data = response.Data;
            if (data == null)
            {
                return null;
            }
            return data.ToJson().ToObject(resultType);
        }
    }

}
