using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XNetCore.STL
{
    public class XContextData
    {
        internal XContextData()
        {
            this.Token = string.Empty;
            this.UserId = string.Empty;
            this.RequestId = string.Empty;
            this.ClientIP = string.Empty;
        }
        #region TraceId
        /// <summary>
        /// 新建Trace
        /// </summary>
        public void NewTraceId()
        {
            this.rpcId = string.Empty;
            this.traceId = Guid.NewGuid().ToString("D");
        }
        /// <summary>
        /// 获取TraceId
        /// </summary>
        /// <returns></returns>
        private string getTraceId()
        {
            var result = this.traceId;
            if (string.IsNullOrWhiteSpace(result))
            {
                NewTraceId();
            }
            result = this.traceId;
            return result;
        }

        private string traceId = string.Empty;
        /// <summary>
        /// 设置或获取TraceId
        /// </summary>
        public string TraceId
        {
            get
            {
                return getTraceId();
            }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    NewTraceId();
                    return;
                }
                if (value.ToLower() == this.traceId.ToString())
                {
                    return;
                }
                this.traceId = value;
                this.rpcId = string.Empty;
            }
        }
        #endregion

        #region RpcId
        /// <summary>
        /// RpcId拆分
        /// </summary>
        /// <param name="rpcId"></param>
        /// <returns></returns>
        private int[] getRpcArray(string rpcId)
        {
            var result = new List<int>();
            if (string.IsNullOrWhiteSpace(rpcId))
            {
                rpcId = string.Empty;
            }
            var ss = rpcId.Split('.');
            if (ss.Length > 10)
            {
                ss = new string[0];
            }
            foreach (var s in ss)
            {
                if (string.IsNullOrWhiteSpace(s))
                {
                    continue;
                }
                int.TryParse(s, out var id);
                result.Add(id);
            }
            if (result.Count == 0)
            {
                result.Add(0);
            }
            return result.ToArray();
        }
        /// <summary>
        /// RpcId拆分
        /// </summary>
        /// <returns></returns>
        private int[] getRpcArray()
        {
            var rpcId = this.rpcId;
            return getRpcArray(rpcId);
        }
        /// <summary>
        /// 新建RpcId
        /// </summary>
        /// <returns></returns>
        private int[] newRpcArray()
        {
            var result = new List<int>();
            result.AddRange(getRpcArray());
            result.Add(0);
            return result.ToArray();
        }
        /// <summary>
        /// RpcId步进
        /// </summary>
        /// <returns></returns>
        private int[] increaseRpcArray()
        {
            var result = getRpcArray();
            result[result.Length - 1]++;
            return result;
        }
        /// <summary>
        /// RpcId格式化
        /// </summary>
        /// <param name="rpcArray"></param>
        /// <returns></returns>
        private string formatRpcId(int[] rpcArray)
        {
            var result = new StringBuilder();
            foreach (var id in rpcArray)
            {
                result.Append(id.ToString()).Append(".");
            }
            result = result.Remove(result.Length - 1, 1);
            this.rpcId = result.ToString();
            return result.ToString();
        }
        /// <summary>
        /// 新建RpcId
        /// </summary>
        public void NewRpcId()
        {
            this.rpcId = formatRpcId(newRpcArray());
        }
        /// <summary>
        /// 步进RpcId
        /// </summary>
        public void IncreaseRpcId()
        {
            this.rpcId = formatRpcId(increaseRpcArray());
        }

        private string rpcId = string.Empty;
        /// <summary>
        /// RpcId
        /// </summary>
        public string RpcId
        {
            get
            {
                return formatRpcId(getRpcArray());
            }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    return;
                }
                formatRpcId(getRpcArray(value));
            }
        }
        #endregion

        #region LogId
        /// <summary>
        /// 获取或设置LogId
        /// </summary>
        public int LogId { get; set; }
        /// <summary>
        /// 步进LogId
        /// </summary>
        public void IncreaseLogId()
        {
            var logId = this.LogId;
            logId++;
            this.LogId = logId;
        }
        #endregion
        /// <summary>
        /// 请求Id
        /// </summary>
        public string RequestId { get; set; }
        /// <summary>
        /// 请求Id
        /// </summary>
        public string ClientIP { get; set; }
        /// <summary>
        /// Token
        /// </summary>
        public string Token { get; set; }
        /// <summary>
        /// UserId
        /// </summary>
        public string UserId { get; set; }

        public XContextData Clone()
        {
            var json = this.ToJson();
            var result = json.ToObject<XContextData>();
            return result;
        }
    }
}
