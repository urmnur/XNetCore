using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XNetCore.CEF.Runner
{
    class PakHelper
    {
        #region 单例模式
        private static object lockobject = new object();
        private static PakHelper _instance = null;
        public static PakHelper Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (lockobject)
                    {
                        if (_instance == null)
                        {
                            _instance = new PakHelper();
                        }
                    }

                }
                return _instance;
            }
        }
        private PakHelper()
        {
        }
        #endregion


        public byte[] GetResource(string demain,string fileName)
        {
            return getResource(demain, fileName);
        }
        private byte[] getResource(string demain, string fileName)
        {
            var fi = new FileInfo(new Uri(this.GetType().Assembly.CodeBase).LocalPath);
            var pakpath = Path.Combine(fi.Directory.FullName, demain + ".pak");
            if (new FileInfo(pakpath).Exists)
            {
                return getZipResource(pakpath, fileName);
            }
            else
            {
                return getFileResource(Path.Combine(fi.Directory.FullName, demain), fileName);
            }
        }
        private byte[] getZipResource(string resource, string fileName)
        {
            fileName = fileName.Trim('/');
            var result = new byte[0];
            ZipEntry zipEntry = null;
            try
            {
                using (ZipInputStream zipStream = new ZipInputStream(File.OpenRead(resource)))
                {
                    while ((zipEntry = zipStream.GetNextEntry()) != null)
                    {
                        string zip_file = zipEntry.Name;
                        MemoryStream decompressedStream = new MemoryStream();
                        byte[] buffer = new byte[2048];
                        if (fileName.ToLower() == zip_file.ToLower())
                        {
                            while (true)
                            {
                                int size = zipStream.Read(buffer, 0, buffer.Length);
                                if (size > 0)
                                {
                                    decompressedStream.Write(buffer, 0, size);
                                }
                                else
                                {
                                    break;
                                }
                            }
                            decompressedStream.Position = 0;
                            int len = (int)decompressedStream.Length;
                            result = new byte[len];
                            decompressedStream.Read(result, 0, len);
                            decompressedStream.Close();
                            decompressedStream.Dispose();
                            return result;
                        }
                    }
                }
                return result;
            }
            catch (System.Exception ex)
            {
            }
            return result;
        }

        private byte[] getFileResource(string resource, string fileName)
        {
            fileName = fileName.Trim('/');
            var result = new byte[0];
            try
            {
                var fi = new FileInfo(Path.Combine(resource, fileName));
                if (!fi.Exists)
                {
                    return result;
                }

                var stream = File.OpenRead(fi.FullName);
                int len = (int)stream.Length;
                result = new byte[len];
                stream.Position = 0;
                stream.Read(result, 0, len);
                stream.Close();
                stream.Dispose();
                return result;
            }
            catch (System.Exception ex)
            {
            }
            return result;
        }

    }
}
