using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XNetCore.STL;

namespace XNetCore.XAPI.Client
{
    public class RegistData
    {
        public string Address { get; set; }
        public int Port { get; set; }
        public string ServiceId { get; set; }
        public string ServiceProvderId { get; set; }
        public string ServiceName { get; set; }
        public string ServiceInfc { get; set; }
        public string ServiceImpl { get; set; }
        public string ServiceMethod { get; set; }
        public string ServiceHandler { get; set; }
        public int Model { get; set; }
        public int Auth { get; set; }
    }

    class ServiceRegistData
    {
        public ServiceRegistData(RegistData data)
        {
            this.RegistData = data;
            if (!string.IsNullOrWhiteSpace(data.ServiceInfc))
            {
                this.ServiceInfc = Type.GetType(data.ServiceInfc, false, true);
            }
            if (!string.IsNullOrWhiteSpace(data.ServiceImpl))
            {
                this.ServiceImpl = Type.GetType(data.ServiceImpl, false, true);
            }
            if (!string.IsNullOrWhiteSpace(data.ServiceHandler))
            {
                this.ServiceHandler = Type.GetType(data.ServiceHandler, false, true);
            }
            if (this.ServiceInfc != null)
            {
                this.RegistData.ServiceInfc = this.ServiceInfc.FullName();
            }
            if (this.ServiceImpl != null)
            {
                this.RegistData.ServiceImpl = this.ServiceImpl.FullName();
            }
            if (this.ServiceHandler != null)
            {
                this.RegistData.ServiceHandler = this.ServiceHandler.FullName();
            }
        }
        public Type ServiceInfc { get;private set; }
        public Type ServiceImpl { get; private set; }
        public Type ServiceHandler { get; private set; }
        public RegistData RegistData { get; private set; }
    }
}
