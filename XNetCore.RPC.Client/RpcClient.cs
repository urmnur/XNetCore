using Castle.DynamicProxy;
using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XNetCore.RPC.Core;
using XNetCore.STL;

namespace XNetCore.RPC.Client
{
    public static class RpcClient
    {
        /// <summary>
        /// 获取Api操作类
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static object GetApi(Type type)
        {
            var result = RpcClientHelper.Instance.GetApi(type);
            return result;
        }
        /// <summary>
        /// 获取Api操作类
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T GetApi<T>()
        {
            var result = RpcClientHelper.Instance.GetApi(typeof(T));
            return (T)result;
        }

        public static ServiceResponse ExcuteService(string serviceName, string param)
        {
            var result = RpcClientHelper.Instance.ExcuteService(serviceName, param);
            return result;
        }


        public static T GetApi<T>(string ip, int port)
        {
            var result = RpcClientHelper.Instance.GetApi(typeof(T), ip, port);
            var data = new RegistData();
            data.Address = ip;
            data.Port = port;
            data.ServiceInfc = typeof(T).FullName();
            RpcClient.Register(data);
            return (T)result;
        }


        public static void Register(RegistData data)
        {
            RpcServiceStore.Instance.RegisterService(data);
        }
        public static void ClearRegister()
        {
            RpcServiceStore.Instance.Clear();
        }

    }
}
