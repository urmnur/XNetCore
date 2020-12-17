using System;
using System.Collections.Generic;
using System.Text;


namespace XNetCore.STL
{
    public  class XHeader
    {
        public const string TOKEN = "X-TOKEN";
        public const string APPID = "X-APPID";
        public const string TRACEID = "X-TRACEID";
        public const string RPCID = "X-RPCID";
        public const string LOGID = "X-LOGID";
        public const string REQUESTID = "X-REQUESTID";
        public const string CLIENTIP = "X-CLIENTIP";
        public const string FileServiceName = "FileService";
        public const string SGServiceName = "SGService";
        public const string DbServiceName = "DbService";


        private  string getHeadersKeys()
        {
            var sb = new StringBuilder();
            sb.Append(APPID).Append(", ");
            sb.Append(TOKEN).Append(", ");
            sb.Append(TRACEID).Append(", ");
            sb.Append(RPCID).Append(", ");
            sb.Append(LOGID).Append(", ");
            sb.Append(REQUESTID).Append(", ");
            sb.Append(CLIENTIP);
            return sb.ToString();
        }

        public  Dictionary<string, string> ResponseHeaders(string origin)
        {
            var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            if (!string.IsNullOrEmpty(origin))
            {
                result.Add("Access-Control-Allow-Origin", origin);
            }
            else
            {
                result.Add("Access-Control-Allow-Origin", "*");
            }
            result.Add("Access-Control-Allow-Methods", "POST,GET,PUT,DELETE,OPTIONS");
            var sb = new StringBuilder();
            sb.Append("Content-Type").Append(",");
            sb.Append("Authorization").Append(",");
            sb.Append(getHeadersKeys()).Append(", ");
            sb.Append("page").Append(", ");
            sb.Append("excel").Append(", ");
            sb.Append("*");
            result.Add("Access-Control-Allow-Headers", sb.ToString());
            result.Add("Access-Control-Allow-Credentials", "true");
            return result;
        }
    }
}
