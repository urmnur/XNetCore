using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using XNetCore.STL;
using Microsoft.AspNetCore.Http;
using Castle.Core.Internal;
using Microsoft.Extensions.Primitives;
using System.Net.Http.Headers;
using System.Text;

namespace XNetCore.XAPI
{
    class KeyValue
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }
    class ApiRequest
    {
        public ApiRequest(Controller controller, string serviceName, string methodName)
        {
            try
            {
                this.Controller = controller;
                this.ServiceName = serviceName;
                this.MethodName = methodName;
                InitializeValue(controller);
            }
            catch { }
        }


        public string IP { get; private set; }
        public string ServiceName { get; private set; }
        public string MethodName { get; private set; }
        public Controller Controller { get; private set; }
        public KeyValue[] KeyValues { get; private set; }
        public KeyValue[] BodyKeyValues { get; private set; }
        public string Body { get; private set; }

        public string GetVaule(string key)
        {
            if (TryGetVaule(key, out string value))
            {
                return value;
            }
            return string.Empty;
        }

        public bool TryGetVaule(string key, out string value)
        {
            if (this.KeyValues != null)
            {
                foreach (var kv in this.KeyValues)
                {
                    if (kv.Key == null)
                    {
                        if (string.IsNullOrWhiteSpace(key))
                        {
                            value = kv.Value;
                            return true;
                        }
                        continue;
                    }
                    if (kv.Key.ToLower() == key.ToLower())
                    {
                        value = kv.Value;
                        return true;
                    }
                }

            }
            if (this.BodyKeyValues != null)
            {
                foreach (var kv in this.BodyKeyValues)
                {
                    if (kv.Key.ToLower() == key.ToLower())
                    {
                        value = kv.Value;
                        return true;
                    }
                }
            }
            value = string.Empty;
            return false;
        }

        #region  InitializeValue
        private void InitializeValue(Controller controller)
        {
            this.Body = getRequestBody(controller);
            var kvs = new List<KeyValue>();
            kvs.AddRange(getRequestParamsData(controller));
            kvs.AddRange(getRequestHeadersData(controller));
            kvs.AddRange(getRequestActionArguments(controller));
            this.KeyValues = kvs.ToArray();
            kvs = new List<KeyValue>();
            kvs.AddRange(getBodyVaule(this.Body));
            this.BodyKeyValues = kvs.ToArray();
            this.IP = getClientIpAddress(controller);
        }



        private KeyValue[] getRequestParamsData(Controller controller)
        {
            var result = new List<KeyValue>();
            var query = controller?.HttpContext?.Request?.Query;
            if (query == null)
            {
                return result.ToArray();
            }

            foreach (var item in query)
            {
                if (string.IsNullOrWhiteSpace(item.Key))
                {
                    continue;
                }
                var value = item.Value;
                if (StringValues.IsNullOrEmpty(value))
                {
                    continue;
                }
                result.Add(new KeyValue() { Key = item.Key, Value = value.ToString() });
            }
            return result.ToArray();
        }
        private KeyValue[] getRequestHeadersData(Controller controller)
        {
            var result = new List<KeyValue>();

            var query = controller?.HttpContext?.Request?.Headers;
            if (query == null)
            {
                return result.ToArray();
            }

            foreach (var item in query)
            {
                if (string.IsNullOrWhiteSpace(item.Key))
                {
                    continue;
                }
                var value = item.Value;
                if (StringValues.IsNullOrEmpty(value))
                {
                    continue;
                }
                result.Add(new KeyValue() { Key = item.Key, Value = value[0] });
            }
            return result.ToArray();
        }
        private KeyValue[] getRequestActionArguments(Controller controller)
        {
            var result = new List<KeyValue>();
            var query = controller?.RouteData?.Values;
            if (query == null)
            {
                return result.ToArray();
            }

            foreach (var item in query)
            {
                if (string.IsNullOrWhiteSpace(item.Key))
                {
                    continue;
                }
                var value = item.Value;
                if (value==null)
                {
                    continue;
                }
                result.Add(new KeyValue() { Key = item.Key, Value = value.ToString() });
            }
            return result.ToArray();
        }

        private string UrlDecode(string str)
        {
            var result = System.Net.WebUtility.UrlDecode(str);
            return result;
        }



        /// <summary>
        /// 获取请求数据 Body
        /// </summary>
        /// <param name="body"></param>
        /// <param name="pName"></param>
        /// <returns></returns>
        private KeyValue[] getBodyVaule(string body)
        {
            var result = new List<KeyValue>();
            if (string.IsNullOrWhiteSpace(body))
            {
                return result.ToArray();
            }
            try
            {
                var b = UrlDecode(body);
                var jobject = JObject.Parse(b);
                foreach (var token in jobject)
                {
                    if (string.IsNullOrWhiteSpace(token.Key))
                    {
                        continue;
                    }
                    var value = token.Value;
                    if (token.Value == null)
                    {
                        continue;
                    }
                    result.Add(new KeyValue() { Key = token.Key, Value = value.ToString() });
                }
            }
            catch { }
            return result.ToArray();
        }

        private string getRequestBody(Controller controller)
        {
            var result = string.Empty;
            try
            {
                var files = getRequestFileBody(controller);
                if (files != null && files.Length > 0)
                {
                    result = files.ToJson();
                    return result;
                }
                var query = controller?.HttpContext?.Request?.Body;
                if (query == null)
                {
                    return result;
                }
                var streamReader = new StreamReader(query);
                result = streamReader.ReadToEnd();
                streamReader.Close();
                streamReader.Dispose();
                result = System.Net.WebUtility.UrlDecode(result);
            }
            catch (Exception ex)
            {
            }
            return result;
        }

        private HttpFileInfo[] getRequestFileBody(Controller controller)
        {
              var ss= controller.HttpContext.Request.ReadFormAsync();

                  var result = new List<HttpFileInfo>();
            //if (!controller.Request.Content.IsMimeMultipartContent())
            //{
            //    return result.ToArray();
            //}
            //var provider = new MultipartMemoryStreamProvider();
            //controller.Request.Content.ReadAsMultipartAsync(provider).Wait();
            //if (provider.Contents == null || provider.Contents.Count == 0)
            //{
            //    return result.ToArray();
            //}
            //foreach (var content in provider.Contents)
            //{
            //    var buffer = content.ReadAsByteArrayAsync();
            //    var item = new HttpFileInfo();
            //    item.Id = Guid.NewGuid().ToString();
            //    item.IP = getClientIpAddress(controller);
            //    item.FileKey = UrlDecode(content.Headers.ContentDisposition.Name).Trim().Trim('"');
            //    item.FileName = UrlDecode(content.Headers.ContentDisposition.FileName).Trim().Trim('"');
            //    item.FileType = content.Headers.ContentType.MediaType;
            //    item.FileLength = (int)content.Headers.ContentLength;
            //    item.FileBase64 = Convert.ToBase64String(buffer.Result);
            //    result.Add(item);
            //}
            return result.ToArray();
        }
        #endregion


        #region 获取客户端IP
        private const string HEADER_HttpContext = "MS_HttpContext";
        private const string HEADER_RemoteEndpointMessage = "System.ServiceModel.Channels.RemoteEndpointMessageProperty";
        private const string HEADER_OwinContext = "MS_OwinContext";
        /// <summary>
        /// 获取客户端IP
        /// </summary>
        /// <param name="controller"></param>
        /// <returns></returns>
        private string getClientIpAddress(Controller controller)
        {
            var result  =  controller.HttpContext.Connection.RemoteIpAddress.ToString();
            return result;
        }
        #endregion

    }
}