using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XNetCore.XAPI.Client;

namespace XNetCore.XAPI.Client
{
    class ClientRegistData :  ServiceRegistData
    {
        public ClientRegistData(RegistData data) : base(data)
        {
        }
    }


}
