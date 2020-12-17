using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using XNetCore.STL;

namespace XNetCore.XRun
{
    public class JWT
    {
        class Payload
        {
            public long exp { get; set; }
            public string user { get; set; }
        }

        private const int const_exp_time_stamp_minutes = 12 * 60;
        private static long gettimestamp(int m)
        {
            var zts = TimeZoneInfo.Local.BaseUtcOffset;
            var yc = new DateTime(1970, 1, 1).Add(zts);
            return (long)(DateTime.Now.AddMinutes(m) - yc).TotalMilliseconds;
        }
        private static string getSign(string data)
        {
            var key = "XNETCORE@JWT";
            var sign = new HMACSHA256(Encoding.UTF8.GetBytes(key))
                .ComputeHash(Encoding.UTF8.GetBytes(data));
            return Convert.ToBase64String(sign);
        }

        public static string CreateToken(string userId)
        {
            var header = "{'typ':'JWT','alg':'HS256'}";
            var payload = new Payload() { exp = gettimestamp(const_exp_time_stamp_minutes), user = userId }.ToJson();
            var result = new StringBuilder();
            result.Append(Convert.ToBase64String(Encoding.UTF8.GetBytes(header))).Append(".");
            result.Append(Convert.ToBase64String(Encoding.UTF8.GetBytes(payload)));
            result.Append(".").Append(getSign(result.ToString()));
            return result.ToString();
        }

        public static string GetUserId(string token)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(token))
                {
                    return string.Empty;
                    throw new XExceptionNoToken(token);
                }
                var ss = token.Split('.');
                if (ss.Length < 3)
                {
                    throw new XExceptionNoFindToken(token);
                }
                var sign = getSign(ss[0] + "." + ss[1]);
                if (sign != ss[2])
                {
                    throw new XExceptionNoFindToken(token);
                }
                //var header = Encoding.UTF8.GetString(Convert.FromBase64String(ss[0]));
                var payload = Encoding.UTF8.GetString(Convert.FromBase64String(ss[1])).ToObject<Payload>();
                if (payload.exp < gettimestamp(0 - const_exp_time_stamp_minutes))
                {
                    throw new XExceptionTokenExpire(token);
                }
                return payload.user;
            }
            catch
            {
                throw new XExceptionNoFindToken(token);
            }
        }
    }

}
