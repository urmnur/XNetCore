using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace XNetCore.XRun
{
    class InvokeData
    {

        public InvokeData()
        {
            this.NameParams = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            this.TypeParams = new List<object>();
        }
        public object Impl { get; set; }
        public MethodInfo Method { get; set; }
        public string ParamsJson { get; set; }
        public Dictionary<string, string> NameParams { get; set; }
        public IList<object>  TypeParams { get; set; }
        public Action<object> CallbackAction { get; set; }
    }
}
