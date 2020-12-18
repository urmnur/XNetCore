using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XNetCore.STL
{
    public class XResponse
    {
        private void iniXResponse()
        {
            this.RequestId = XContext.Current.RequestId;
            this.Code = 200;
            this.TraceId = XContext.Current.TraceId;
            this.RpcId = XContext.Current.RpcId;
            this.LogId = XContext.Current.LogId;
        }
        public XResponse()
        {
            iniXResponse();
        }
        public XResponse(object rsf)
        {
            iniXResponse();
            this.Data = rsf;
        }


        public XResponse(Exception ex)
        {
            iniXResponse();
            this.Code = 99999;
            if (ex != null)
            {
                this.Error = ex.ToString();
                var xex = ex;
                for (var i = 0; i < 5; i++)
                {
                    if (xex is XException)
                    {
                        var h = xex as XException;
                        this.Error = h.ToString();
                        this.Code = h.ErrCode;
                        this.Msg = string.Empty + h.ErrMsg;
                        this.Data = h.ErrData;
                        break;
                    }
                    xex = xex.InnerException;
                    if (xex == null)
                    {
                        break;
                    }
                }
            }
        }
        public string RequestId { get; set; }
        public string ServiceId { get; set; }
        public string TraceId { get; set; }
        public string RpcId { get; set; }
        public int LogId { get; set; }
        public int Code { get; set; }
        public string Msg { get; set; }
        public string Error { get; set; }
        public object Data { get; set; }
    }
}
