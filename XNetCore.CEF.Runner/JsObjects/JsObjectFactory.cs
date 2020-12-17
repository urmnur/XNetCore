using CefSharp;
using XNetCore.CEF.Runner;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace XNetCore.CEF.Runner
{
    internal class JsObjectFactory
    {
        #region 单例模式
        private static object lockobject = new object();
        private static JsObjectFactory _instance = null;
        public static JsObjectFactory Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (lockobject)
                    {
                        if (_instance == null)
                        {
                            _instance = new JsObjectFactory();
                        }
                    }

                }
                return _instance;
            }
        }
        private JsObjectFactory()
        {
        }
        #endregion

        public JsObject[] AllJsObjects
        {
            get
            {
                return getAllJsObjects();
            }
        }
        private JsObject[] getAllJsObjects()
        {
            var result = new List<JsObject>();
            result.AddRange(getAllJsObjects(this.GetType().Assembly));
            return result.ToArray();
        }

        private JsObject[] getAllJsObjects(Assembly assembly)
        {
            var result = new List<JsObject>();
            if (!assembly.FullName.ToLower().StartsWith(this.GetType().FullName.Substring(0, this.GetType().FullName.IndexOf(".")).ToLower()))
            {
                return result.ToArray();
            }
            var types = assembly.GetTypes();
            foreach (var type in types)
            {
                try
                {
                    var obj = getAllJsObject(type);
                    if (obj != null)
                    {
                        result.Add(obj);
                    }
                }
                catch { }
            }
            return result.ToArray();
        }
        private JsObject getAllJsObject(Type type)
        {
            if (type == null)
            {
                return null;
            }

            if (!typeof(BrowserService).IsAssignableFrom(type))
            {
                return null;
            }
            var instance = (BrowserService)System.Activator.CreateInstance(type);
            var name = getName(type);
            var result = new JsObjectImpl() { Name = name, Instance = instance };
            return result;
        }

        private string getName(Type type)
        {
            var result = type.Name.ToLower();
            return result;
        }
    }
}
