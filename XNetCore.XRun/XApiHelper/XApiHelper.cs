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
    class XApiHelper
    {
        #region 单例模式
        private static object lockobject = new object();
        private static XApiHelper _instance = null;
        public static XApiHelper Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (lockobject)
                    {
                        if (_instance == null)
                        {
                            _instance = new XApiHelper();
                        }
                    }

                }
                return _instance;
            }
        }
        private XApiHelper()
        {
        }
        #endregion

        public XResponse Run(string typeName, string methodName, string param, string token)
        {
            try
            {
                var result = mRun(typeName, methodName, param, token);
                return new XResponse(result);
            }
            catch (Exception ex)
            {
                return new XResponse(ex);
            }
        }
        private string urlDeCode(string str)
        {
            var encoding = Encoding.Default;
            Encoding utf8 = Encoding.UTF8;
            //首先用utf-8进行解码                    
            string code = HttpUtility.UrlDecode(str.ToUpper(), utf8);
            //将已经解码的字符再次进行编码.
            string encode = HttpUtility.UrlEncode(code, utf8).ToUpper();
            if (str == encode)
                encoding = Encoding.UTF8;
            return HttpUtility.UrlDecode(str, encoding);
        }
        private object mRun(string typeName, string methodName, string param, string token)
        {
            typeName = urlDeCode(typeName).Trim();
            methodName = urlDeCode(methodName).Trim();
            var userid = JWT.GetUserId(token);
            XContext.Current.UserId = userid;
            var result = InvokeImpl.Instance.Invoke(typeName.ToType(), methodName, param,null,null,null);
            return result;
        }
    }
}
