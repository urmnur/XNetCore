using CefSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XNetCore.CEF.Runner
{
    interface XSchemeHandler : ISchemeHandlerFactory
    {

    }
    interface CustomScheme
    {
        string DomainName { get; }
        XSchemeHandler Instance { get; }
    }


    internal class CustomSchemeImpl : CustomScheme
    {
        public string DomainName { get; set; }
        public XSchemeHandler Instance { get; set; }
    }
}
