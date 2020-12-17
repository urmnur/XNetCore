using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XNetCore.AppTest.Runner
{
    class ATest
    {
        #region 单例模式
        private static object lockobject = new object();
        private static ATest _instance = null;
        public static ATest Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (lockobject)
                    {
                        if (_instance == null)
                        {
                            _instance = new ATest();
                        }
                    }

                }
                return _instance;
            }
        }

        private ATest()
        {
            this.ABC = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }
        #endregion


        public string ABC { get; private set; }
    }

    class BTest
    {
        public string getAbc()
        {
            return ATest.Instance.ABC;
        }
    }
}
