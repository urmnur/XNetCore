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
    class JsonExtendHelper
    {
        #region 单例模式
        private static object lockobject = new object();
        private static JsonExtendHelper _instance = null;
        public static JsonExtendHelper Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (lockobject)
                    {
                        if (_instance == null)
                        {
                            _instance = new JsonExtendHelper();
                        }
                    }

                }
                return _instance;
            }
        }
        private JsonExtendHelper()
        {
        }
        #endregion

        #region ToJson
        /// <summary>
        /// 获取CamelCase明明
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private string getCamelCaseName(string name)
        {
            var result = new StringBuilder();
            var ss = name.Split('_');
            var idx = 0;
            foreach (var str in ss)
            {
                if (string.IsNullOrWhiteSpace(str))
                {
                    continue;
                }
                idx++;
                if (idx == 1)
                {
                    result.Append(str.Substring(0, 1).ToLower());
                }
                else
                {
                    result.Append(str.Substring(0, 1).ToUpper());
                }
                result.Append(str.Substring(1));
            }
            return result.ToString();
        }
        private string objToJson(object obj, Formatting formatting, string datetimeformats)
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
                DateFormatString = datetimeformats,
                DateParseHandling = DateParseHandling.None,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                NullValueHandling = NullValueHandling.Ignore,
            };
            return JsonConvert.SerializeObject(obj, formatting, setting);
        }

        /// <summary>
        /// 转化为Json
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="datetimeformats"></param>
        /// <returns></returns>
        public string ToJson(object obj, Formatting formatting, string datetimeformats)
        {
            if (obj == null)
            {
                return null;
            }
            if (obj is DataTable)
            {
                var data = (obj as DataTable).Copy();
                foreach (DataColumn dc in data.Columns)
                {
                    dc.ColumnName = getCamelCaseName(dc.ColumnName);
                }
                obj = data;
            }
            return objToJson(obj, formatting, datetimeformats);
        }

        private string objToCamelCaseJson(object obj, Formatting formatting, string datetimeformats)
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
                DateFormatString = datetimeformats,
                DateParseHandling = DateParseHandling.None,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                NullValueHandling = NullValueHandling.Ignore,
            };
            setting.Converters.Add(new JsonTokenCamelcasePropertyNameConverter());
            return JsonConvert.SerializeObject(obj, formatting, setting);
        }
        /// <summary>
        /// 转化为Json
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="datetimeformats"></param>
        /// <returns></returns>
        public string ToCamelCaseJson(object obj, Formatting formatting, string datetimeformats)
        {
            if (obj == null)
            {
                return null;
            }
            if (obj is DataTable)
            {
                var data = (obj as DataTable).Copy();
                foreach (DataColumn dc in data.Columns)
                {
                    dc.ColumnName = getCamelCaseName(dc.ColumnName);
                }
                obj = data;
            }
            return objToCamelCaseJson(obj, formatting, datetimeformats);
        }


        
        #endregion

        #region ToJObject
        private string trimString(string value)
        {
            var result = value;
            if (string.IsNullOrWhiteSpace(result))
            {
                result = string.Empty;
            }
            result = result.Replace("&nbsp;", "");
            result = result.Trim().Trim('\r').Trim('\n').Trim();
            return result;
        }

        /// <summary>
        /// 转化为实例
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public JObject ToJObject(string json)
        {
            var str = trimString(json);
            return JObject.Parse(str);
        }

        private object toObject(string json)
        {
            var str = trimString(json);
            if (str.StartsWith("["))
            {
                return JArray.Parse(str);
            }
            return JObject.Parse(str);
        }
        /// <summary>
        /// 转化为实例
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public object ToObject(string json)
        {
            return toObject(json);
        }
        #endregion

        #region ToObject
        /// <summary>
        /// 转化为实例
        /// </summary>
        /// <param name="json"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        private object toObject(string json, Type type)
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                return null;
            }
            if (type.IsSimpleType())
            {
                return  Convert.ChangeType(json, type);
            }
            return string.IsNullOrWhiteSpace(json) ? null : JsonConvert.DeserializeObject(json, type);
        }
        /// <summary>
        /// 转化为实例
        /// </summary>
        /// <param name="json"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public object ToObject(string json, Type type)
        {
            try
            {

                var value = toObject(json, type);
                if (value == null)
                {
                    value = type.DefaultValue();
                }
                return value;
            }
            catch (Exception ex)
            {
                throw new Exception($"{json}\r\n>>>>>>>>>>>>>>>>>>>>>>>\r\n{type.FullName}\r\n---------------\r\n{ex.ToString()}");
            }
        }
        #endregion
    }
}
