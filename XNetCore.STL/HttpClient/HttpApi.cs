using System;
using System.Collections.Generic;
using System.Text;

namespace XNetCore.STL
{
    public static class HttpApi
    {
        /// <summary>
        /// 获取Api操作类
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static object GetApi(Type type, string url = null)
        {
            var result = ApiHelper.Instance.GetApi(type, url);
            return result;
        }
        /// <summary>
        /// 获取Api操作类
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T GetApi<T>(string url = null)
        {
            var result = ApiHelper.Instance.GetApi(typeof(T), url);
            return (T)result;
        }

        public static HResponse Get(string url, Dictionary<string, string> header = null, int timeout = 10000)
        {
            return HttpHelper.Instance.Get(url, header, timeout);
        }
        /// <summary>
        /// 执行Http Post 请求
        /// </summary>
        /// <param name="url"></param>
        /// <param name="header"></param>
        /// <param name="body"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public static HResponse Post(string url, Dictionary<string, string> header = null, object body = null, int timeout = 10000)
        {
            return HttpHelper.Instance.Post(url, header, body, timeout);
        }

    }
}
