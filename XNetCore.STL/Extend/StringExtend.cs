using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XNetCore.STL
{
    /// <summary>
    /// 字符串扩展
    /// </summary>
    public static class StringExtend
    {
        /// <summary>
        /// 转化为Int
        /// </summary>
        /// <param name="str"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static string Null2Empty(this string str)
        {
            if (str==null)
            {
                return string.Empty;
            }
            return str;
        }
        /// <summary>
        /// 转化为Int
        /// </summary>
        /// <param name="str"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static int ToInt(this string str,int defaultValue)
        {
            var result = defaultValue;
            if (string.IsNullOrWhiteSpace(str))
            {
                return defaultValue;
            }
            if (int.TryParse(str.Trim(), out result))
            {
                return result;
            }
            return defaultValue;
        }
        /// <summary>
        /// 转化为Decimal
        /// </summary>
        /// <param name="str"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static decimal ToDecimal(this string str, decimal defaultValue)
        {
            var result = defaultValue;
            if (string.IsNullOrWhiteSpace(str))
            {
                return defaultValue;
            }
            if (decimal.TryParse(str.Trim(), out result))
            {
                return result;
            }
            return defaultValue;
        }
        /// <summary>
        /// 转化为DateTime
        /// </summary>
        /// <param name="str"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static DateTime ToDateTime(this string str, DateTime defaultValue)
        {
            var result = defaultValue;
            if (string.IsNullOrWhiteSpace(str))
            {
                return defaultValue;
            }
            if (DateTime.TryParse(str.Trim(), out result))
            {
                return result;
            }
            return defaultValue;
        }
    }
}
