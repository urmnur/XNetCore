using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using XNetCore.STL;

namespace XNetCore.XRun
{
    class InvokeImpl
    {
        #region 单例模式
        private static object lockobject = new object();
        private static InvokeImpl _instance = null;
        public static InvokeImpl Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (lockobject)
                    {
                        if (_instance == null)
                        {
                            _instance = new InvokeImpl();
                        }
                    }

                }
                return _instance;
            }
        }
        private InvokeImpl()
        {
        }
        #endregion


        private Type getInstanceType(object impl)
        {
            if (impl == null)
            {
                throw new XNetCore.STL.XExceptionNoFindImpl("Invoke Impl is null");
            }
            if (impl.GetType().IsSubclassOf(typeof(Type)))
            {
                return impl as Type;
            }
            if (impl.GetType().IsSubclassOf(typeof(string)))
            {
                return impl.ToString().ToType();
            }
            return null;
        }

        private object getInstance(object impl, string instanceName)
        {
            var type = getInstanceType(impl);
            if (type != null)
            {
                try
                {
                    return InvokeType.Instance.GetInstance(type, instanceName);
                }
                catch
                {
                    return type;
                }
            }
            return impl;
        }



        private (string instanceName, string methodName) getMethodInstance(string method)
        {
            if (string.IsNullOrWhiteSpace(method))
            {
                throw new XNetCore.STL.XExceptionNoFindImpl("Invoke Method is null");
            }
            var idx = method.IndexOf("|");
            if (idx < 0)
            {
                idx = method.IndexOf(",");
            }
            if (idx < 0)
            {
                idx = method.IndexOf(".");
            }
            if (idx < 0)
            {
                return ("", method);
            }
            var instanceName = "." + method.Substring(0, idx);
            var methodName = method.Substring(idx + 1);
            return (instanceName, methodName);
        }


        public object Invoke(object impl, string methodName, string param, Dictionary<string, string> nameParams, IList<object> typeParams, Action<object> callback)
        {
            var m = getMethodInstance(methodName);
            var result = mInvoke(getInstance(impl, m.instanceName), m.methodName, param, nameParams, typeParams, callback);
            return result;
        }

        private object mInvoke(object impl, string methodName, string param, Dictionary<string, string> nameParams, IList<object> typeParams, Action<object> callback)
        {
            var data = new InvokeData();
            data.Impl = impl;
            data.Method = getMethod(data.Impl, methodName);
            if (param != null)
            {
                data.ParamsJson = param;
            }
            data.NameParams = createNameParams();
            if (nameParams != null)
            {
                foreach (var k in nameParams.Keys)
                {
                    if (data.NameParams.ContainsKey(k))
                    {
                        data.NameParams[k] = nameParams[k];
                        continue;
                    }
                    var v = nameParams[k];
                    data.NameParams.Add(k, v);
                }
            }
            if (typeParams != null)
            {
                data.TypeParams = typeParams;
            }
            if (callback != null)
            {
                data.CallbackAction = callback;
            }
            var result = InvokeRunner.Instance.InvokeMethod(data);
            return result;
        }

        private Dictionary<string, string> createNameParams()
        {
            var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            if (XContext.Current == null)
            {
                return result;
            }
            var type = XContext.Current.GetType();
            var ps = type.GetProperties();
            foreach (var p in ps)
            {
                var k = p.Name;
                var v = p.GetValue(XContext.Current);
                if (v == null)
                {
                    continue;
                }
                result.Add(k, v.ToString());
                if (k.ToLower() == "ClientIP".ToLower())
                {
                    result.Add("ip", v.ToString());
                }
            }
            type = XApp.Current.GetType();
            ps = type.GetProperties();
            foreach (var p in ps)
            {
                var k = p.Name;
                var v = p.GetValue(XApp.Current);
                if (v == null)
                {
                    continue;
                }
                result.Add(k, v.ToString());
            }
            return result;
        }
        private MethodInfo getMethod(object impl, string methodName)
        {
            var type = impl.GetType();
            if (impl.GetType().IsSubclassOf(typeof(Type)))
            {
                type = impl as Type;
            }
            var methods = type.GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            MethodInfo method1 = null;
            MethodInfo method2 = null;
            MethodInfo method3 = null;

            foreach (var method in methods)
            {
                if (method.Name.ToLower() == methodName.ToLower())
                {
                    if (method.Name == methodName)
                    {
                        method1 = method;
                        return method1;
                    }
                    else if (method.IsPublic)
                    {
                        method2 = method;
                    }
                    method3 = method;
                }
            }
            if (method1 != null)
            {
                return method1;
            }
            if (method2 != null)
            {
                return method2;
            }
            if (method3 != null)
            {
                return method3;
            }
            throw new XNetCore.STL.XExceptionNoFindMethod($"{type.FullName}.{methodName}");
        }
    }
}
