using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace XNetCore.XAPI
{
    public class SGServiceController : Controller
    {
        [HttpGet, HttpPut, HttpPost, HttpDelete]
        public object DefaultAction(string serviceName, string methodName)
        {
            var result = DefaultActionHelper.Instance.Run(this, serviceName, methodName);
            return result;
        }
    }
}
