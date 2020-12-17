using CefSharp;
using CefSharp.WinForms;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace XNetCore.CEF.Runner
{
    class ClientHelper
    {
        #region 单例模式
        private static object lockobject = new object();
        private static ClientHelper _instance = null;
        public static ClientHelper Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (lockobject)
                    {
                        if (_instance == null)
                        {
                            _instance = new ClientHelper();
                        }
                    }

                }
                return _instance;
            }
        }

        private ClientHelper()
        {
            CoreHelper.Instance.AddressChanged += onAddressChanged;
        }
        #endregion

        public Control MainFrmControl { get { return CoreHelper.Instance.MainFrmControl; } }

        public event Action<string> AddressChanged;
        private void onAddressChanged(string arg)
        { 
            this.AddressChanged?.Invoke(arg);
        }


        public Icon favicon { get { return CoreHelper.Instance.favicon; } }

        public void Shutdown()
        {
            try { Cef.Shutdown(); } catch { }
        }
        public bool ApplicationExit { get; set; }
    }
}