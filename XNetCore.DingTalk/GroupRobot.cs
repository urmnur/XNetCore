using System;
using System.Collections.Generic;
using System.Text;
using XNetCore.STL;

namespace XNetCore.DingTalk
{
    public class GroupRobot
    {
        private string roboturl = string.Empty;
        private string secret = string.Empty;
        public GroupRobot(string url, string secret)
        {
            this.roboturl = url;
            this.secret = secret;
        }


        public HResponse SendMsg(DingTalkMsg msg)
        {
            var url = this.roboturl;
            if (!string.IsNullOrWhiteSpace(this.secret))
            {
                var timestamp = gettimestamp();
                var stringToSign = $"{timestamp}\n{this.secret}";
                var sign = hash_hmac2(stringToSign, this.secret);
                sign = System.Web.HttpUtility.UrlEncode(sign);
                url = url + $"&timestamp={timestamp}&sign={sign}";
            }
            var response = HttpApi.Post(url, null, msg, 0);
            return response;
        }


        private long gettimestamp()
        {
            var zts = TimeZoneInfo.Local.BaseUtcOffset;
            var yc = new DateTime(1970, 1, 1).Add(zts);
            return (long)(DateTime.Now - yc).TotalMilliseconds;
        }
        private string hash_hmac2(string message, string secret)
        {
            secret = secret ?? "";
            var encoding = new System.Text.ASCIIEncoding();
            byte[] keyByte = encoding.GetBytes(secret);
            byte[] messageBytes = encoding.GetBytes(message);
            using (var hmacsha256 = new System.Security.Cryptography.HMACSHA256(keyByte))
            {
                byte[] hashmessage = hmacsha256.ComputeHash(messageBytes);
                return Convert.ToBase64String(hashmessage);
            }
        }
    }
}
