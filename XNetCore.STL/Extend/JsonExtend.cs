using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XNetCore.STL
{
    /// <summary>
    /// Json扩展
    /// </summary>
    public static class JsonExtend
    {
        #region ToJson

        private const string default_datetimeformats = "yyyy-MM-dd HH:mm:ss.fff";

        /// <summary>
        /// 转化为Json
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string ToJson(this object obj)
        {
            return JsonExtendHelper.Instance.ToJson(obj, Formatting.None, default_datetimeformats);
        }

        /// <summary>
        /// 转化为Json
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="datetimeformats"></param>
        /// <returns></returns>
        public static string ToJson(this object obj, string datetimeformats)
        {
            return JsonExtendHelper.Instance.ToJson(obj, Formatting.None, datetimeformats);
        }
        /// <summary>
        /// 转化为Json
        /// </summary>
        /// <param name="dTable"></param>
        /// <returns></returns>
        public static string ToJson(this DataTable dTable)
        {
            return JsonExtendHelper.Instance.ToJson(dTable, Formatting.None, default_datetimeformats);
        }
        /// <summary>
        /// 转化为Json
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="datetimeformats"></param>
        /// <returns></returns>
        public static string ToIndentedJson(this object obj)
        {
            return JsonExtendHelper.Instance.ToJson(obj, Formatting.Indented, default_datetimeformats);
        }
        /// <summary>
        /// 转化为Json
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="datetimeformats"></param>
        /// <returns></returns>
        public static string ToIndentedJson(this object obj, string datetimeformats)
        {
            return JsonExtendHelper.Instance.ToJson(obj, Formatting.Indented, datetimeformats);
        }
        /// <summary>
        /// 转化为Json
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="datetimeformats"></param>
        /// <returns></returns>
        public static string ToCamelCaseJson(this object obj)
        {
            return JsonExtendHelper.Instance.ToCamelCaseJson(obj, Formatting.Indented, default_datetimeformats);
        }
        /// <summary>
        /// 转化为Json
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="datetimeformats"></param>
        /// <returns></returns>
        public static string ToJsResponse(this object obj)
        {
            if (obj == null)
            {
                return null;
            }
            if (obj.GetType() == typeof(string))
            {
                return obj.ToString();
            }
            if (obj.GetType().IsSimpleType())
            {
                return obj.ToString();
            }
            var setting = new JsonSerializerSettings
            { 
                ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver(),
                DateFormatString = default_datetimeformats,
                DateParseHandling = DateParseHandling.None,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                NullValueHandling = NullValueHandling.Include,
            };
            setting.Converters.Add(new DBNullConverter());
            setting.Converters.Add(new JsonTokenCamelcasePropertyNameConverter());
            return JsonConvert.SerializeObject(obj, Formatting.Indented, setting);
        }
        /// <summary>
        /// 转化为Json
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="datetimeformats"></param>
        /// <returns></returns>
        public static string ToCamelCaseJson(this object obj, string datetimeformats)
        {
            return JsonExtendHelper.Instance.ToCamelCaseJson(obj, Formatting.Indented, datetimeformats);
        }
        #endregion

        #region ToJObject
        /// <summary>
        /// 转化为实例
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public static JObject ToJObject(this string json)
        {
            return JsonExtendHelper.Instance.ToJObject(json);
        }
        /// <summary>
        /// 转化为实例
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public static object ToObject(this string json)
        {
            return JsonExtendHelper.Instance.ToObject(json);
        }

        #endregion

        #region ToObject
        /// <summary>
        /// 转化为实例
        /// </summary>
        /// <param name="json"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static object ToObject(this string json, Type type)
        {
            return JsonExtendHelper.Instance.ToObject(json, type);
        }
        /// <summary>
        /// 转化为实例
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json"></param>
        /// <returns></returns>
        public static T ToObject<T>(this string json)
        {
            var type = typeof(T);
            var result =  JsonExtendHelper.Instance.ToObject(json, type);
            if (result==null)
            {
                return default(T);
            }
            return (T)result;
        }
        /// <summary>
        /// 转化为实例列
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json"></param>
        /// <returns></returns>
        public static List<T> ToList<T>(this string json)
        {
            var type = typeof(List<T>);
            var result = JsonExtendHelper.Instance.ToObject(json, type);
            if (result == null)
            {
                return default(List<T>);
            }
            return (List<T>)result;
        }
        /// <summary>
        /// 转化为DataTable
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public static DataTable ToTable(this string json)
        {
            var type = typeof(DataTable);
            var result = JsonExtendHelper.Instance.ToObject(json, type);
            if (result == null)
            {
                return new DataTable();
            }
            return result as DataTable;
        }
        #endregion
    }
}
