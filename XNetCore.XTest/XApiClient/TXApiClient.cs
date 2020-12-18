using System;
using System.Collections.Generic;
using System.Text;
using XNetCore.XAPI.Client;

namespace XNetCore.XTest
{
    class TXApiClient
    {
        public void MyTest()
        {
            XApiClient.GetResponse<object>("127.0.0.1", 1024, "XNetCore.CEF.Runner.CefApp,XNetCore.CEF.Runner", "showdevtools", null);
        }

    }
}
