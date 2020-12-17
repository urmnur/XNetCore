using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XNetCore.KApp
{
    class Md5Helper
    {
        #region 单例模式
        private static readonly object lockobject = new object();
        private static Md5Helper _instance;
        public static Md5Helper Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (lockobject)
                    {
                        if (_instance == null)
                        {
                            _instance = new Md5Helper();
                        }
                    }

                }
                return _instance;
            }
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        private Md5Helper()
        {
        }

        #endregion

        /// <summary>
        /// 获取文件MD5值
        /// </summary>
        /// <param name="fileName">文件绝对路径</param>
        /// <returns>MD5值</returns>
        public string GetFileMd5(string fileName)
        {
            try
            {
                var fi = new FileInfo(fileName);
                if (!fi.Exists)
                {
                    return string.Empty;
                }
                var file = new FileStream(fileName, FileMode.Open, FileAccess.Read);
                System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
                byte[] retVal = md5.ComputeHash(file);
                file.Close();
                var sb = new StringBuilder();
                for (int i = 0; i < retVal.Length; i++)
                {
                    sb.Append(retVal[i].ToString("x2"));
                }
                return sb.ToString();
            }
            catch (Exception ex)
            {
            }
            return string.Empty;
        }
    }
}
