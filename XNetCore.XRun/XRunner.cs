using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using XNetCore.STL;

namespace XNetCore.XRun
{
    public static class XRunner
    {
        public static XResponse Api(string typeName, string methodName, string param, string token)
        {
           return  XApiHelper.Instance.Run( typeName,  methodName,  param,  token);
        }

        public static object Invoke(object impl, string methodName, string param, Dictionary<string, string> nameParams, IList<object> typeParams, Action<object> callback)
        {
            var result = InvokeImpl.Instance.Invoke(impl, methodName, param, nameParams, typeParams, callback);
            return result;
        }

        public static object Js(string script)
        {
            return JsRunner.Instance.ExecuteScript(script);
        }
        public static void JsAsync(string script)
        {
             JsRunner.Instance.ExecuteScriptAsync(script);
        }
    }
}
