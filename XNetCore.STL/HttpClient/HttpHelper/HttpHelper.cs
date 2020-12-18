using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;

namespace XNetCore.STL
{
    /// <summary>
    /// HttpClient操作
    /// </summary>
    public class HttpHelper
    {
        #region 单例模式
        private static object lockobject = new object();
        private static HttpHelper _instance = null;
        public static HttpHelper Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (lockobject)
                    {
                        if (_instance == null)
                        {
                            _instance = new HttpHelper();
                        }
                    }

                }
                return _instance;
            }
        }
        private HttpHelper()
        {
        }
        #endregion
        /// <summary>
        /// 执行Http请求
        /// </summary>
        /// <param name="url"></param>
        /// <param name="method"></param>
        /// <param name="headers"></param>
        /// <param name="body"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        private HResponse http(string url, string method, Dictionary<string, string> headers, object body, int timeout)
        {
            var result = new HResponse();
            if (string.IsNullOrWhiteSpace(url))
            {
                throw new Exception("Url IsNullOrWhiteSpace");
            }
            result.StatusCode = HttpStatusCode.BadRequest;
            var resResultString = string.Empty;
            if (timeout < 1000)
            {
                timeout = 10000;
            }
            try
            {
                HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(url);
                if (url.ToLower().StartsWith("https"))
                {
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls;
                }
                if (headers != null)
                {
                    foreach (var kv in headers)
                    {
                        req.Headers.Add(kv.Key, kv.Value);
                    }
                }
                req.Timeout = timeout;
                req.Method = method;
                var buff = new byte[0];
                var bodyData = new StringBuilder();
                if (body != null)
                {
                    if (body is HForm[] || body is HForm)
                    {
                        req.ContentType = "application/x-www-form-urlencoded";
                        var forms = new List<HForm>();
                        if (body is HForm[])
                        {
                            forms.AddRange(body as HForm[]);
                        }
                        if (body is HForm)
                        {
                            forms.Add(body as HForm);
                        }
                        foreach (var form in forms)
                        {
                            if (string.IsNullOrWhiteSpace(form.Key))
                            {
                                continue;
                            }
                            bodyData.Append(HttpUtility.HtmlEncode(form.Key)).Append("=").Append(HttpUtility.HtmlEncode(form.Value)).Append("&");
                        }
                        if (bodyData.ToString().EndsWith("&"))
                        {
                            bodyData.Remove(bodyData.Length - 1, 1);
                        }
                    }
                    else
                    {
                        if (body != null && body is Dictionary<string, object>)
                        {
                            var dic = body as Dictionary<string, object>;
                            if (dic != null && dic.Values.Count == 1)
                            {
                                body = dic.Values.FirstOrDefault();
                            }
                        }
                        req.ContentType = "application/json;charset=utf-8";
                        bodyData = new StringBuilder();
                        bodyData.Append(body.ToJson());
                    }
                }
                if (bodyData.Length > 0)
                {
                    buff = Encoding.UTF8.GetBytes(bodyData.ToString());
                }
                req.ContentLength = buff.Length;
                if (buff.Length > 0)
                {
                    using (Stream reqStream = req.GetRequestStream())
                    {
                        reqStream.Write(buff, 0, buff.Length);
                    }
                }
                using (HttpWebResponse res = (HttpWebResponse)req.GetResponse())
                {
                    result.StatusCode = res.StatusCode;
                    Stream resStream = res.GetResponseStream();
                    if (resStream != null)
                    {
                        StreamReader reader = new StreamReader(resStream, System.Text.Encoding.GetEncoding("utf-8"));
                        resResultString = reader.ReadToEnd();
                        reader.Close();
                        resStream.Close();
                        req.Abort();
                        res.Close();
                    }
                }
            }
            catch (Exception ex)
            {

            }
            result.ResponseData = resResultString;
            return result;
        }
        /// <summary>
        /// 执行Http Get 请求
        /// </summary>
        /// <param name="url"></param>
        /// <param name="header"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public HResponse Get(string url, Dictionary<string, string> header, int timeout)
        {
            return this.http(url, "GET", header, null, timeout);
        }
        /// <summary>
        /// 执行Http Post 请求
        /// </summary>
        /// <param name="url"></param>
        /// <param name="header"></param>
        /// <param name="body"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public HResponse Post(string url, Dictionary<string, string> header, object body, int timeout)
        {
            return this.http(url, "POST", header, body, timeout);
        }
    }

}
