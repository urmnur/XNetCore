using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using XNetCore.STL;

namespace XNetCore.XAPI.Client
{
    /// <summary>
    /// 远程服务
    /// </summary>
    class XApiService
    {
        #region 单例模式
        private static object lockobject = new object();
        private static XApiService _instance = null;
        public static XApiService Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (lockobject)
                    {
                        if (_instance == null)
                        {
                            _instance = new XApiService();
                        }
                    }

                }
                return _instance;
            }
        }
        private XApiService()
        {
        }
        #endregion

        public XResponse ExcuteService(ClientRegistData service, string methodName, string request)
        {
            var url = getUrl(service, methodName);
            var header = getHeader();
            var body = request;
            var timeout = 10 * 1000;
            var result =  HttpHelper.Instance.Post(url, header, body, timeout);
            return getResult(result);
        }

        private string getUrl(ClientRegistData service, string methodName)
        {
            if (string.IsNullOrWhiteSpace(service?.RegistData?.Address))
            {
                return string.Empty;
            }
            var result = new StringBuilder();
            result.Append("http://");
            result.Append(service.RegistData.Address);
            if (service.RegistData.Port > 0)
            {
                result.Append(":").Append(service.RegistData.Port.ToString());
            }
            result.Append("/").Append(service.RegistData.ServiceImpl);
            if (string.IsNullOrWhiteSpace(methodName))
            {
                result.Append("/").Append(service.RegistData.ServiceMethod);
            }
            else
            {
                result.Append("/").Append(methodName);
            }
            return result.ToString();
        }

        private Dictionary<string, string> getHeader()
        {
            var result = new Dictionary<string, string>();
            try
            {
                result.Add(XHeader.TOKEN, XContext.Current.Token);
                result.Add(XHeader.TRACEID, XContext.Current.TraceId);
                result.Add(XHeader.RPCID, XContext.Current.RpcId);
                result.Add(XHeader.LOGID, XContext.Current.LogId.ToString());
                result.Add(XHeader.REQUESTID, XContext.Current.RequestId);
                result.Add(XHeader.CLIENTIP, XContext.Current.ClientIP);
            }
            catch { }
            return result;
        }
        private XResponse getResult(HResponse response)
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
            var result = data.ToObject<XResponse>();
            return result;
        }

    }

}
