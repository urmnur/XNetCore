using System;
using System.Collections.Generic;
using System.Text;

namespace XNetCore.STL
{
    [System.AttributeUsage(System.AttributeTargets.Method, AllowMultiple = true)]
    public class HttpApiAttribute : Attribute
    {
        public HttpMethod Method { get; set; }
        public HttpContentType ContentType { get; set; }
        public string ServiceName { get; set; }
        public string MethodName { get; set; }
        public int Timeout { get; set; }
    }
    [System.AttributeUsage(System.AttributeTargets.Interface, AllowMultiple = true)]
    public class HttpServiceAttribute : Attribute
    {
        public string BaseUrl { get; set; }
        public string ServiceName { get; set; }
        public int Timeout { get; set; }
    }

    public enum HttpMethod
    {
        None,
        Get,
        Post,
    }
    public enum HttpContentType
    {
        None,
        Json,
        Form,
    }
}
