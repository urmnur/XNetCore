using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace XNetCore.STL
{
    /// <summary>
    /// 字符串扩展
    /// </summary>
    public static class NamingConventionExtend
    {
        public static string TrimLine(this string str)
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                return string.Empty;
            }
            var lastIsSpace = false;
            var chars = new List<char>();
            foreach (var c in str.ToArray())
            {
                if (c == ' ')
                {
                    if (lastIsSpace)
                    {
                        continue;
                    }
                    lastIsSpace = true;
                }
                else
                {
                    lastIsSpace = false;
                }
                if (c == '\r' || c == '\n')
                {
                    continue;
                }
                chars.Add(c);
            }
            return new string(chars.ToArray()).Trim();
        }
        /// <summary>
        /// 转换为Pascal命名法
        /// </summary>
        /// <param name="str">要转换的字符串</param>
        /// <returns>返回转换后的字符串</returns>
        public static string ToPascal(this string str)
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                return string.Empty;
            }
            if (str.Trim().ToLower() == "objid")
            {
                return "ObjId";
            }
            const char c_c = '_';
            var hasUpper = false;
            var hasLower = false;
            var chars = new List<char>();
            foreach (var c in str.ToArray())
            {
                switch (c)
                {
                    case '_':
                        chars.Add(c_c);
                        break;
                    case '.':
                        chars.Add(c_c);
                        break;
                    case ',':
                        chars.Add(c_c);
                        break;
                    case '-':
                        chars.Add(c_c);
                        break;
                    case '|':
                        chars.Add(c_c);
                        break;
                    case '\r':
                        chars.Add(c_c);
                        break;
                    case '\n':
                        chars.Add(c_c);
                        break;
                    case ' ':
                        chars.Add(c_c);
                        break;
                    default:
                        chars.Add(c);
                        break;
                }
                if (c <= 122 && c >= 97)
                {
                    hasLower = true;
                }
                else if (c <= 90 && c >= 65)
                {
                    hasUpper = true;
                }
            }
            str = new string(chars.ToArray());
            StringBuilder result = new StringBuilder();
            string[] ss = str.Split(c_c);
            foreach (var s in ss)
            {
                if (string.IsNullOrWhiteSpace(s))
                {
                    continue;
                }
                result.Append(s.Substring(0, 1).ToUpper());
                if (hasLower && hasUpper)
                {
                    result.Append(s.Substring(1));
                }
                else
                {
                    result.Append(s.Substring(1).ToLower());
                }
            }
            return result.ToString();
        }
        /// <summary>
        /// 转换为Pascal命名法
        /// </summary>
        /// <param name="str">要转换的字符串</param>
        /// <returns>返回转换后的字符串</returns>
        public static string ToCamelCase(this string str)
        {
            var result = str.ToPascal();
            if (string.IsNullOrWhiteSpace(result))
            {
                return result;
            }
            return result.Substring(0, 1).ToLower() + result.Substring(1);
        }
        /// <summary>
        /// 转换为Pascal命名法
        /// </summary>
        /// <param name="str">要转换的字符串</param>
        /// <returns>返回转换后的字符串</returns>
        public static string to_underscores(this string str)
        {
            var result = str.ToPascal();
            if (string.IsNullOrWhiteSpace(result))
            {
                return result;
            }
            var chars = new List<char>();
            foreach (var c in result.ToArray())
            {
                if (c <= 90 && c >= 65)
                {
                    if (chars.Count > 0)
                    {
                        chars.Add('_');
                    }
                    chars.Add((char)((byte)c + 32));
                }
                else
                {
                    chars.Add(c);
                }
            }
            return new string(chars.ToArray());
        }

    }
}
