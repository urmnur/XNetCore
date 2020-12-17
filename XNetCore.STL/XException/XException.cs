using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XNetCore.STL
{
    /// <summary>
    /// 异常类
    /// </summary>
    public class XException : Exception
    {
        public XException() : base() { }
        public XException(string message) : base(message) { }
        public XException(string message, Exception innerException) : base(message, innerException) { }

        /// <summary>
        /// 异常编号 
        /// </summary>
        public int ErrCode { get; set; }
        /// <summary>
        /// 异常消息
        /// </summary>
        public string ErrMsg { get; set; }
        /// <summary>
        /// 异常数据
        /// </summary>
        public object ErrData { get; set; }
    }

    public class XExceptionNoFindService : XException
    {
        public XExceptionNoFindService(string name) : base() { iniException(name); }
        public XExceptionNoFindService(string name, string message) : base(message) { iniException(name); }
        public XExceptionNoFindService(string name, string message, Exception innerException) : base(message, innerException) { iniException(name); }
        private void iniException(string name)
        {
            this.ErrCode = 5000;
            this.ErrMsg = $"获取服务失败[{name}]";
        }
    }
    public class XExceptionNoFindInfc : XException
    {
        public XExceptionNoFindInfc(string name) : base() { iniException(name); }
        public XExceptionNoFindInfc(string name, string message) : base(message) { iniException(name); }
        public XExceptionNoFindInfc(string name, string message, Exception innerException) : base(message, innerException) { iniException(name); }
        private void iniException(string name)
        {
            this.ErrCode = 5001;
            this.ErrMsg = $"获取接口失败[{name}]";
        }
    }
    public class XExceptionNoFindImpl : XException
    {
        public XExceptionNoFindImpl(string name) : base() { iniException(name); }
        public XExceptionNoFindImpl(string name, string message) : base(message) { iniException(name); }
        public XExceptionNoFindImpl(string name, string message, Exception innerException) : base(message, innerException) { iniException(name); }
        private void iniException(string name)
        {
            this.ErrCode = 5002;
            this.ErrMsg = $"获取服务失败[{name}]";
        }
    }
    public class XExceptionImplInstance : XException
    {
        public XExceptionImplInstance(Type type) : base() { iniException(type); }
        public XExceptionImplInstance(Type type, string message) : base(message) { iniException(type); }
        public XExceptionImplInstance(Type type, string message, Exception innerException) : base(message, innerException) { iniException(type); }
        private void iniException(Type type)
        {
            this.ErrCode = 5021;
            this.ErrMsg = $"获取服务失败[{type.FullName()}]";
        }
    }
    public class XExceptionNoFindMethod : XException
    {
        public XExceptionNoFindMethod(string name) : base() { iniException(name); }
        public XExceptionNoFindMethod(string name, string message) : base(message) { iniException(name); }
        public XExceptionNoFindMethod(string name, string message, Exception innerException) : base(message, innerException) { iniException(name); }
        private void iniException(string name)
        {
            this.ErrCode = 5003;
            this.ErrMsg = $"获取方法失败[{name}]";
        }
    }
    public class XExceptionNoToken : XException
    {
        public XExceptionNoToken() : base() { iniException(); }
        public XExceptionNoToken( string message) : base(message) { iniException(); }
        public XExceptionNoToken( string message, Exception innerException) : base(message, innerException) { iniException(); }
        private void iniException()
        {
            this.ErrCode = 5100;
            this.ErrMsg = $"Token为空";
        }
    }
    public class XExceptionNoFindToken : XException
    {
        public XExceptionNoFindToken(string token) : base() { iniException(token); }
        public XExceptionNoFindToken(string token, string message) : base(message) { iniException(token); }
        public XExceptionNoFindToken(string token, string message, Exception innerException) : base(message, innerException) { iniException(token); }
        private void iniException(string token)
        {
            this.ErrCode = 5101;
            this.ErrMsg = $"Token无效[{token}]";
        }
    }
    public class XExceptionTokenExpire : XException
    {
        public XExceptionTokenExpire(string token) : base() { iniException(token); }
        public XExceptionTokenExpire(string token, string message) : base(message) { iniException(token); }
        public XExceptionTokenExpire(string token, string message, Exception innerException) : base(message, innerException) { iniException(token); }
        private void iniException(string token)
        {
            this.ErrCode = 5102;
            this.ErrMsg = $"Token过期[{token}]";
        }
    }
}
