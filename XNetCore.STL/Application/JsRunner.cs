using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XNetCore.STL;

namespace XNetCore.STL
{
    public class JsRunner
    {
        #region 单例模式
        private static object lockobject = new object();
        private static JsRunner _instance = null;
        public static JsRunner Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (lockobject)
                    {
                        if (_instance == null)
                        {
                            _instance = new JsRunner();
                        }
                    }

                }
                return _instance;
            }
        }
        private JsRunner()
        {
        }
        #endregion


        private object runner = null;

        private object getRunner()
        {
            var type = Type.GetType("XNetCore.CEF.Core.JsRunner, XNetCore.CEF.Core", false, true);
            if (type == null)
            {
                return null;
            }
            var p = Activator.CreateInstance(type, null);
            if (p == null)
            {
                return null;
            }
            return p;
        }

        private object mExecuteScriptMethod(string method,object[] args)
        {
            if (this.runner == null)
            {
                this.runner = getRunner();
            }
            if (this.runner == null)
            {
                return null;
            }
            var type = this.runner.GetType();
            foreach (var m in type.GetMethods())
            {
                if (m.Name.ToLower() == method.ToLower())
                {
                    return m.Invoke(this.runner, args);
                }
            }
            return null;
        }


        public object ExecuteScript(string script)
        {
            return mExecuteScriptMethod("ExecuteScript", new object[] { script });
        }

        public void ExecuteScriptAsync(string script)
        {
            mExecuteScriptMethod("ExecuteScriptAsync", new object[] { script });
        }
    }
}
