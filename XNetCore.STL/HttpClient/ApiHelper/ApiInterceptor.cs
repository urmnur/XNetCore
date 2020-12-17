using Castle.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Diagnostics.SymbolStore;
using System.IO;
using System.Linq;
using System.Reflection;

namespace XNetCore.STL
{
    /// <summary>
    /// RPC操作拦截器
    /// </summary>
    class ApiInterceptor : IInterceptor
    {
        private string url;

        public ApiInterceptor(string url)
        {
            this.url = url;
        }

        /// <summary>
        /// 拦截器执行
        /// </summary>
        /// <param name="invocation"></param>
        public void Intercept(IInvocation invocation)
        {
            InterceptExcute(invocation);
        }

        private void InterceptExcute(IInvocation invocation)
        {
            //DeclaringType：声明方法的类；ReflectedType：当前的反射类
            var method = invocation.Method;
            if (method == null || method.DeclaringType == (Type)null)
            {
                return;
            }
            invocation.ReturnValue = ApiMethodHelper.Instance.HttpApi(this.url, method, invocation.Arguments);
        }

    }

}
