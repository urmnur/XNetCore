using CefSharp;
using System;
using System.IO;

namespace XNetCore.CEF.Runner
{
    class PackScheme
    {
        private static object lockobject = new object();
        public IResourceHandler Create(IBrowser browser, IFrame frame, string schemeName, IRequest request)
        {
            lock (lockobject)
            {
                return mCreate(browser, frame, schemeName, request);
            }
        }
        private IResourceHandler mCreate(IBrowser browser, IFrame frame, string schemeName, IRequest request)
        {
            var uri = new Uri(request.Url);
            var filePath = uri.AbsolutePath;
            if (filePath.StartsWith("/"))
            {
                filePath = filePath.Substring(1);
            }
            if (filePath.Contains("?"))
            {
                filePath = filePath.Substring(0, filePath.IndexOf("?"));
            }
            var fileExtension = Path.GetExtension(filePath);
            var mimeType = ResourceHandler.GetMimeType(fileExtension);
            var domainName = uri.Host;
            var buff = PakHelper.Instance.GetResource(domainName, filePath);
            var stream = new MemoryStream();
            stream.Write(buff, 0, buff.Length);
            stream.Position = 0;
            var result = ResourceHandler.FromStream(stream, mimeType);
            result.AutoDisposeStream = true;
            return result;
        }
    }
    class UIScheme : PackScheme, XSchemeHandler
    {
    }
    class CDNScheme : PackScheme, XSchemeHandler
    {
    }
    class VueScheme : PackScheme, XSchemeHandler
    {
    }
}