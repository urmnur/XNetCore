
using XNetCore.STL;
using XNetCore.XRun;

namespace XNetCore.CEF.Runner
{
    interface BrowserService
    {
    }
    class XBrowser : BrowserService
    {
        public string run(string typeName, string methodName,  string param, string token)
        {
            return XRunner.Api(typeName, methodName, param, token).ToJsResponse();
        }
    }
}