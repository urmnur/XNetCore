using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using XNetCore.STL;

namespace XNetCore.XRun
{
    class InvokeType
    {
        #region 单例模式
        private static object lockobject = new object();
        private static InvokeType _instance = null;
        public static InvokeType Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (lockobject)
                    {
                        if (_instance == null)
                        {
                            _instance = new InvokeType();
                        }
                    }

                }
                return _instance;
            }
        }
        private InvokeType()
        {
        }
        #endregion

        public object GetInstance(Type type, string instanceName)
        {
            if (!string.IsNullOrWhiteSpace(instanceName)
                && (instanceName.Contains(".")))
            {
                var idx = instanceName.IndexOf(".");
                return getImplInstance1(type, instanceName.Substring(idx + 1));
            }
            return getImplInstance2(type, instanceName);
        }

        private object getImplInstance2(Type type, string instanceName)
        {
            var result = createImplInstance(type, instanceName);
            if (result == null)
            {
                throw new XNetCore.STL.XExceptionImplInstance(type);
            }
            return result;
        }
        private ConcurrentDictionary<Type, object> implement_objs = new ConcurrentDictionary<Type, object>();
        private object getImplInstance1(Type type, string instanceName)
        {
            if (type == null)
            {
                throw new XNetCore.STL.XExceptionNoFindImpl("Type is null");
            }
            if (implement_objs.ContainsKey(type))
            {
                implement_objs.TryGetValue(type, out object impl);
                return impl;
            }
            lock (lockobject)
            {
                if (implement_objs.ContainsKey(type))
                {
                    implement_objs.TryGetValue(type, out object impl);
                    return impl;
                }
                var result = createImplInstance(type, instanceName);
                if (result == null)
                {
                    throw new XNetCore.STL.XExceptionImplInstance(type);
                }
                implement_objs.TryAdd(type, result);
                return result;
            }
        }
        private object createImplInstance(Type type, string instanceName)
        {
            if (string.IsNullOrWhiteSpace(instanceName))
            {
                return createTypeImplInstance(type);
            }
            var p = type.GetProperty(instanceName);
            if (p == null)
            {
                return null;
            }
            var result = p.GetValue(null, null);
            return result;
        }
        private object createTypeImplInstance(Type type)
        {
            if (type.IsAbstract && type.IsSealed)
            {
                return type;
            }
            var result = type.Instance();
            return result;
        }
    }
}
