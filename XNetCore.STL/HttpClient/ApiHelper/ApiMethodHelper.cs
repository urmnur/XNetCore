using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using XNetCore.STL;

namespace XNetCore.STL
{
    class ApiMethodHelper
    {
        #region 单例模式
        private static object lockobject = new object();
        private static ApiMethodHelper _instance = null;
        public static ApiMethodHelper Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (lockobject)
                    {
                        if (_instance == null)
                        {
                            _instance = new ApiMethodHelper();
                        }
                    }

                }
                return _instance;
            }
        }
        private ApiMethodHelper()
        {
        }
        #endregion
        public object HttpApi(string baseUrl, MethodInfo method, object[] param)
        {
            var response = httpApi(baseUrl, method, getHeader(method, param), getBody(method, param));
            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                throw new XException(response.ToString());
            }
            return getResult(method, response);
        }

        private object getResult(MethodInfo method, HResponse response)
        {
            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                throw new XException(response.ToString());
            }
            if (string.IsNullOrWhiteSpace(response.ResponseData))
            {
                throw new XException(response.ToString());
            }
            var data = response.ResponseData;
            if (string.IsNullOrWhiteSpace(data))
            {
                return null;
            }
            if (method.ReturnType == null)
            {
                return null;
            }
            var api = data.ToObject<ApiResponse>();
            if (api.Data == null)
            {
                return null;
            }
            var result = api.Data.ToJson().ToObject(method.ReturnType);
            return result;
        }

        private bool paramIsHeader(ParameterInfo p)
        {
            if (p == null)
            {
                return false;
            }
            var pname = p.Name.ToLower();
            return pname == "header";
        }

        private Dictionary<string, string> getHeader(MethodInfo method, object[] param)
        {
            var result = new Dictionary<string, string>();
            var ps = method.GetParameters();
            for (int i = 0; i < ps.Length; i++)
            {
                var p = ps[i];
                if (!paramIsHeader(p))
                {
                    continue;
                }
                var value = param[i];
                if (value == null)
                {
                    return null;
                }
                if (value is Dictionary<string, string>)
                {
                    return value as Dictionary<string, string>;
                }
                return getHeader(value);
            }
            if (result.Count < 1)
            {
                return null;
            }
            return result;
        }

        private Dictionary<string, string> getHeader(object header)
        {
            if (header == null)
            {
                return null;
            }
            var result = new Dictionary<string, string>();
            var ps = header.GetType().GetProperties();
            foreach (var p in ps)
            {
                var value = p.GetValue(header, null);
                if (value != null)
                {
                    result.Add(p.Name, value.ToJson());
                }
            }
            return result;
        }
        private Dictionary<string, object> getBody(MethodInfo method, object[] param)
        {
            var result = new Dictionary<string, object>();
            var ps = method.GetParameters();
            for (int i = 0; i < ps.Length; i++)
            {
                var p = ps[i];
                if (paramIsHeader(p))
                {
                    continue;
                }
                var value = param[i];
                result.Add(p.Name, value);
            }
            if (result.Count < 1)
            {
                return null;
            }
            return result;
        }

        private HResponse httpApi(string baseUrl, MethodInfo method, Dictionary<string, string> header, Dictionary<string, object> param)
        {
            var sattr = getHttpServiceAttribute(method);
            var mattr = getHttpMethodAttribute(method);
            var url = getUrl(baseUrl, sattr);
            var serviceName = getServiceName(sattr, mattr).Trim();
            var methodName = getMethodName(method.Name, mattr).Trim();
            url = getUrl(url, serviceName, methodName);
            var timeout = 10000;
            if (sattr.Timeout > 0)
            {
                timeout = sattr.Timeout;
            }
            if (mattr.Timeout > 0)
            {
                timeout = mattr.Timeout;
            }
            var httpmethod = getHttpMethod(method, mattr.Method);
            if (httpmethod == HttpMethod.Get)
            {
                return HttpHelper.Instance.Get(url, header, timeout);
            }
            var contentType = getHttpContentType(method, mattr.ContentType);
            var body = getBody(param, contentType);
            return HttpHelper.Instance.Post(url, header, body, timeout);
        }


        private HttpApiAttribute getHttpMethodAttribute(MethodInfo method)
        {
            var attrs = method.GetCustomAttributes();
            foreach (var attr in attrs)
            {
                if (attr == null)
                {
                    continue;
                }
                if (attr is HttpApiAttribute)
                {
                    return attr as HttpApiAttribute;
                }
            }
            return null;
        }

        private HttpServiceAttribute getHttpServiceAttribute(MethodInfo method)
        {
            var attrs = method.DeclaringType.GetCustomAttributes();
            foreach (var attr in attrs)
            {
                if (attr == null)
                {
                    continue;
                }
                if (attr is HttpServiceAttribute)
                {
                    return attr as HttpServiceAttribute;
                }
            }
            return null;
        }
        private string getUrl(string url, HttpServiceAttribute attr)
        {
            if (!string.IsNullOrWhiteSpace(url))
            {
                return url;
            }
            if (attr == null || string.IsNullOrWhiteSpace(attr.BaseUrl))
            {
                return string.Empty;
            }
            return attr.BaseUrl;
        }
        private string getServiceName(HttpServiceAttribute sattr, HttpApiAttribute mattr)
        {
            if (mattr != null && !string.IsNullOrWhiteSpace(mattr.ServiceName))
            {
                return mattr.ServiceName;
            }
            if (sattr != null && !string.IsNullOrWhiteSpace(sattr.ServiceName))
            {
                return sattr.ServiceName;
            }
            return string.Empty;
        }
        private string getMethodName(string methodName, HttpApiAttribute mattr)
        {
            if (mattr != null && !string.IsNullOrWhiteSpace(mattr.MethodName))
            {
                return mattr.MethodName;
            }
            return methodName;
        }



        private string getUrl(string url, string serviceName, string methodName)
        {
            var result = new StringBuilder();
            if (string.IsNullOrWhiteSpace(url) && string.IsNullOrWhiteSpace(serviceName))
            {
                return null;
            }
            url = url.Trim();
            if (url.EndsWith("/"))
            {
                url = url.Substring(0, url.Length - 1);
            }
            result.Append(url);
            if (!string.IsNullOrWhiteSpace(serviceName))
            {
                if (serviceName.EndsWith("/"))
                {
                    serviceName = serviceName.Substring(0, serviceName.Length - 1);
                }
                if (string.IsNullOrWhiteSpace(result.ToString()))
                {
                    result.Append(serviceName);
                }
                else
                {
                    result.Append("/").Append(serviceName);
                }
            }
            if (!string.IsNullOrWhiteSpace(methodName))
            {
                result.Append("/").Append(methodName);
            }
            return result.ToString();
        }


        private HttpMethod getHttpMethod(MethodInfo method, HttpMethod httpmethod)
        {
            if (httpmethod != HttpMethod.None)
            {
                return httpmethod;
            }
            return HttpMethod.Post;
        }


        private HttpContentType getHttpContentType(MethodInfo method, HttpContentType contentType)
        {
            if (contentType != HttpContentType.None)
            {
                return contentType;
            }
            if (method == null)
            {
                return 0;
            }
            var paramCount = 0;
            var ps = method.GetParameters();
            for (int i = 0; i < ps.Length; i++)
            {
                var p = ps[i];
                if (paramIsHeader(p))
                {
                    continue;
                }
                var thisCount = p.ParameterType.GetProperties().Length;
                if (thisCount > 0)
                {
                    return HttpContentType.Json;
                }
                paramCount++;
            }
            if (paramCount > 1)
            {
                return HttpContentType.Json;
            }
            return HttpContentType.Form;
        }

        private object getBody(Dictionary<string, object> param, HttpContentType contentType)
        {
            if (param == null)
            {
                return null;
            }
            if (contentType == HttpContentType.Form)
            {
                var result = getFormBody(param);
                if (result == null || result.Length == 0)
                {
                    return null;
                }
                return result;
            }
            return param;
        }
        private HForm[] getFormBody(Dictionary<string, object> param)
        {
            if (param == null)
            {
                return null;
            }
            var result = new List<HForm>();
            foreach (var key in param.Keys.ToArray())
            {
                if (string.IsNullOrWhiteSpace(key))
                {
                    continue;
                }
                result.Add(new HForm()
                {
                    Key = key,
                    Value = param[key].ToJson(),
                });
            }
            return result.ToArray();
        }
    }
}
