using CefSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using XNetCore.STL;
using XNetCore.XRun;

namespace XNetCore.CEF.Runner
{
    class ApiScheme : XSchemeHandler
    {
        public IResourceHandler Create(IBrowser browser, IFrame frame, string schemeName, IRequest request)
        {
            var result = getResourceHandler(request);
            if (request.Method.ToUpper() == "OPTIONS")
            {
                return result;
            }
            result.AutoDisposeStream = true;
            result.Stream = getStream(getActionResult(request));
            return result;
        }
        #region Request


        private string getActionResult(IRequest request)
        {
            var result = actionExcute(request);
            return result.ToJsResponse();
        }
        private XResponse actionExcute(IRequest request)
        {
            var serviceName = getServiceName(request, 1);
            var methodName = getServiceName(request, 2);
            var param = getRequestBody(request);
            var token = getRequestToken(request, XHeader.TOKEN);
            return XRunner.Api(serviceName, methodName, param, token);
        }




        private string getRequestBody(IRequest request)
        {
            var result = string.Empty;
            if (request.PostData != null
                && request.PostData.Elements.Count > 0
                && request.PostData.Elements[0] != null)
            {
                result = Encoding.UTF8.GetString(request.PostData.Elements[0].Bytes);
            }
            return result;
        }
        private string getRequestToken(IRequest request, string k)
        {
            var result = string.Empty;
            if (request.Headers != null
                && request.Headers.Count > 0)
            {
                for (var i = 0; i < request.Headers.Count; i++)
                {
                    var key = request.Headers.AllKeys[i];
                    if (key == null || string.IsNullOrWhiteSpace(key)
                        || !key.Equals(k, StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }
                    var values = request.Headers.GetValues(key);
                    if (values == null || values.Length == 0)
                    {
                        continue;
                    }
                    return values[0];
                }
            }
            return result;
        }


        const int CONST_URL_SCHEME_INDEX = 3;

        private string getServiceName(IRequest request, int idx)
        {
            var url = request.Url;
            for (var i = 0; i < CONST_URL_SCHEME_INDEX + idx - 1; i++)
            {
                url = url.Substring(url.IndexOf('/') + 1);
            }
            var uIdx = url.IndexOf('/');
            if (uIdx > 0)
            {
                url = url.Substring(0, uIdx);
            }
            return url;
        }

        private Stream getStream(string str)
        {
            var buff = Encoding.UTF8.GetBytes(str);
            var result = new MemoryStream();
            result.Write(buff, 0, buff.Length);
            result.Position = 0;
            return result;
        }
        #endregion

        #region Header
        private void responseAppendHeader(ResourceHandler handler, string key, string value)
        {
            foreach (var hk in handler.Headers.AllKeys)
            {
                if (hk != null && hk.ToLower() == key.ToLower())
                {
                    handler.Headers.Set(hk, value);
                    return;
                }
            }
            handler.Headers.Add(key, value);

            //if (string.IsNullOrWhiteSpace(request.Headers[key]))
            //{
            //    handler.Headers.Add(key, value);
            //}
        }

        private ResourceHandler getResourceHandler(IRequest request)
        {
            var result = new ResourceHandler();
            string origin = request.Headers["Origin"];
            var headers = new XHeader().ResponseHeaders(origin);
            foreach (var kv in headers)
            {
                responseAppendHeader(result, kv.Key, kv.Value);
            }
            return result;
        }
        #endregion
    }
}