using System;
using XNetCore.STL;

namespace XNetCore.XAPI.Client
{
    public static class XApiClient
    {
        /// <summary>
        /// 获取Api操作类
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static object GetApi(Type type)
        {
            var result = XApiClientHelper.Instance.GetApi(type);
            return result;
        }
        /// <summary>
        /// 获取Api操作类
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T GetApi<T>()
        {
            var result = XApiClientHelper.Instance.GetApi(typeof(T));
            return (T)result;
        }

        public static T GetResponse<T>(string ip, int port, string serverName, string methodName, string param)
        {
            Register(new RegistData()
            {
                Address = ip,
                Port = port,
                ServiceInfc = serverName,
                ServiceImpl = serverName,
            });
            var service = XApiServiceStore.Instance.GetServiceByIntfc(serverName, methodName);
            if (service == null)
            {
                throw new XNetCore.STL.XExceptionNoFindInfc($"{serverName}.{methodName}");
            }
            var response = XApiService.Instance.ExcuteService(service, methodName, param);
            if (typeof(T).FullName()==response.GetType().FullName())
            {
                return (T)(object)response;
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
            if (response.Data == null || string.IsNullOrWhiteSpace(response.Data.ToJson()))
            {
                return default(T);
            }
            return response.Data.ToJson().ToObject<T>();
        }

        public static void Register(RegistData data)
        {
            XApiServiceStore.Instance.RegisterService(data);
        }
        public static void ClearRegister()
        {
            XApiServiceStore.Instance.Clear();
        }

    }
}
