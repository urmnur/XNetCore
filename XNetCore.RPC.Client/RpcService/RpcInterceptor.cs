using Castle.DynamicProxy;
using Grpc.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using XNetCore.RPC.Core;
using XNetCore.STL;

namespace XNetCore.RPC.Client
{
    /// <summary>
    /// RPC操作拦截器
    /// </summary>
    internal class RpcInterceptor : IInterceptor
    {

        public RpcInterceptor()
        {
        }

        private string rpcIp = string.Empty;
        private int rpcPort = 0;

        public RpcInterceptor(string ip, int port)
        {
            this.rpcIp = ip;
            this.rpcPort = port;
        }

        /// <summary>
        /// 拦截器执行
        /// </summary>
        /// <param name="invocation"></param>
        public void Intercept(IInvocation invocation)
        {
            InterceptExcute(invocation);
        }

        private ClientRegistData getRpcServiceDataByIntfc(MethodInfo method)
        {
            var typeName = method.DeclaringType.FullName();
            var result = RpcServiceStore.Instance.GetServiceByIntfc(typeName, method.Name);
            if (result == null)
            {
                throw new XNetCore.STL.XExceptionNoFindInfc($"{typeName}.{method.Name}");
            }
            return result;
        }



        private bool isMethod(object obj)
        {
            if (obj == null)
            {
                return false;
            }
            return obj.GetType().IsAssignableFrom(typeof(Action<object>));
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
            var isStreamService = false;
            Delegate action = null;
            for (int i = 0; i < ps.Length; i++)
            {
                var value = invocation.Arguments[i];
                if (isMethod(value))
                {
                    action = (Delegate)value;
                    isStreamService = true;
                    continue;
                }
                param.Add(ps[i].Name, value);
            }
            var request = new ServiceRequest();
            request.ServiceData = new RequestServiceData();
            request.ServiceData.ServiceMethod = method.Name;
            request.ServiceData.ServiceParam = param.ToJson();
            var servicedata = getRpcServiceDataByIntfc(method);
            request = new RequestHelper().GetServiceRequest(servicedata, request);
            var channel = getChannel(servicedata);
            if (isStreamService)
            {
                RpcStream.Instance.ExcuteService(servicedata, channel, request, action);
            }
            else
            {
                var response = RpcService.Instance.ExcuteService(servicedata, channel, request);
                invocation.ReturnValue = getRpcResult(response, method.ReturnType);
            }
        }


        private Channel getChannel(ClientRegistData service)
        {
            return service.CreateChannel(rpcIp, rpcPort);
        }


        /// <summary>
        /// 获取Api结果
        /// </summary>
        /// <param name="response"></param>
        /// <param name="resultType"></param>
        /// <returns></returns>
        private object getRpcResult(ServiceResponse response, Type resultType)
        {
            if (response?.ServiceData == null)
            {
                return null;
            }
            if (!string.IsNullOrWhiteSpace(response.ServiceData.ServiceError))
            {
                var msg = response.ServiceData.ServiceError;
                if (!string.IsNullOrWhiteSpace(response.ServiceData.ServiceMsg))
                {
                    msg = response.ServiceData.ServiceMsg;
                }
                throw new XNetCore.STL.XException(msg)
                {
                    ErrCode = response.ServiceData.ServiceCode,
                    ErrData = response.ServiceData.ServiceData,
                    ErrMsg = msg,
                };
            }
            var data = response.ServiceData.ServiceData;
            if (data == null)
            {
                return null;
            }
            return data.ToString().ToObject(resultType);
        }
    }

}
