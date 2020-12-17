using CefSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using XNetCore.STL;

namespace XNetCore.CEF.Runner
{
    internal class CustomSchemeFactory
    {
        #region 单例模式
        private static object lockobject = new object();
        private static CustomSchemeFactory _instance = null;
        public static CustomSchemeFactory Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (lockobject)
                    {
                        if (_instance == null)
                        {
                            _instance = new CustomSchemeFactory();
                        }
                    }

                }
                return _instance;
            }
        }
        private CustomSchemeFactory()
        {
        }
        #endregion
        public CustomScheme[] AllSchemes
        {
            get
            {
                return getAllSchemes();
            }
        }
        private CustomScheme[] getAllSchemes()
        {
            var result = new List<CustomScheme>();
            result.AddRange( getAllSchemes(this.GetType().Assembly));
            return result.ToArray();
        }
        private CustomScheme[] getAllSchemes(Assembly assembly)
        {
            var result = new List<CustomScheme>();

            if (!assembly.FullName.ToLower().StartsWith(this.GetType().FullName.Substring(0, this.GetType().FullName.IndexOf(".")).ToLower()))
            {
                return result.ToArray();
            }
            var types = assembly.GetTypes();
            foreach (var type in types)
            {
                try
                {
                    var scheme = getAllScheme(type);
                    if (scheme != null)
                    {
                        result.Add(scheme);
                    }
                }
                catch { }
            }
            return result.ToArray();
        }
        private CustomScheme getAllScheme(Type type)
        {
            if (type == null)
            {
                return null;
            }
            if (!typeof(XSchemeHandler).IsAssignableFrom(type))
            {
                return null;
            }
            var instance = (XSchemeHandler)System.Activator.CreateInstance(type);
            var domainName = getDomainName(type);
            var result = new CustomSchemeImpl() { DomainName = domainName, Instance = instance };
            return result;
        }

        private string getDomainName(Type type)
        {
            var result = type.FullName;
            var spName = this.GetType().Namespace;
            foreach (var s in spName.Split('.'))
            {
                result = result.Replace(s, string.Empty);
            }
            result = result.Replace("Scheme", string.Empty);
            result = spName.Substring(0, spName.IndexOf('.')) + "." + result;
            while (true)
            {
                var idx = result.IndexOf("..");
                if (idx < 0)
                {
                    break;
                }
                result = result.Replace("..", ".");
            }
            return result;
        }
    }
}
