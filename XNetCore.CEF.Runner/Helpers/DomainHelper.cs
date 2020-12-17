using CefSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace XNetCore.CEF.Runner
{
    internal class DomainHelper
    {
        #region 单例模式
        private static object lockobject = new object();
        private static DomainHelper _instance = null;
        public static DomainHelper Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (lockobject)
                    {
                        if (_instance == null)
                        {
                            _instance = new DomainHelper();
                        }
                    }

                }
                return _instance;
            }
        }
        private DomainHelper()
        {
        }
        #endregion


        public string GetName(Type type)
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