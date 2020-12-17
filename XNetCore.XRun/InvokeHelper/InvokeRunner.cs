using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using XNetCore.STL;

namespace XNetCore.XRun
{
    class InvokeRunner
    {
        #region 单例模式
        private static object lockobject = new object();
        private static InvokeRunner _instance = null;
        public static InvokeRunner Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (lockobject)
                    {
                        if (_instance == null)
                        {
                            _instance = new InvokeRunner();
                        }
                    }

                }
                return _instance;
            }
        }
        private InvokeRunner()
        {
        }
        #endregion

        public object InvokeMethod(InvokeData data)
        {
            var args = getMetodParametersValues(data);
            if (data.Method.IsStatic)
            {
                var result = data.Method.Invoke(null, args);
                return result;
            }
            else
            {
                var result = data.Method.Invoke(data.Impl, args);
                return result;
            }


        }
        private ParamsData getMetod1Param(InvokeData data)
        {
            var parameters = data.Method.GetParameters();
            var result = new ParamsData();
            var param1Name = string.Empty;
            result.ParamsCount = parameters.Length;
            foreach (var parameter in parameters)
            {
                if (isActionParameter(parameter))
                {
                    result.ParamsCount--;
                    continue;
                }
                try
                {
                    var flag = getMetodParameterValueByName(parameter, data.NameParams, out object value);
                    if (flag)
                    {
                        result.ParamsCount--;
                        continue;
                    }
                }
                catch (Exception ex)
                {
                }
                try
                {
                    var flag = getMetodParameterValueByType(parameter, data.TypeParams, out object value);
                    if (flag)
                    {
                        result.ParamsCount--;
                        continue;
                    }
                }
                catch (Exception ex)
                {
                }
                param1Name = parameter.Name;
            }

            if (result.ParamsCount == 1)
            {
                result.Param1Name = param1Name;
            }

            return result;
        }

        class ParamsData
        {
            public int ParamsCount { get; set; }
            public string Param1Name { get; set; }

        }

        private object[] getMetodParametersValues(InvokeData data)
        {
            var paramData = getMetod1Param(data);
            var parameters = data.Method.GetParameters();
            var result = new List<object>();
            foreach (var parameter in parameters)
            {
                if (isActionParameter(parameter))
                {
                    result.Add(data.CallbackAction);
                    continue;
                }
                result.Add(getMetodParametersValues(parameter, data, paramData));
            }
            return result.ToArray();
        }

        private object getMetodParametersValues(ParameterInfo parameter, InvokeData data, ParamsData paramsData)
        {
            var value = getMetodParametersValuesByInvoke(parameter, data, paramsData);
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
            {
                value = getTypeDefaultValue(parameter);
            }
            return value;
        }

        private object getMetodParametersValuesByInvoke(ParameterInfo parameter, InvokeData data, ParamsData paramsData)
        {
            try
            {
                var flag = getMetodParameterValue(parameter, data.ParamsJson, out object value);
                if (flag && value!=null && !string.IsNullOrWhiteSpace(value.ToString()))
                {
                    return value;
                }
            }
            catch (Exception ex)
            {
            }
            try
            {
                var flag = getMetodParameterValueByName(parameter, data.NameParams, out object value);
                if (flag)
                {
                    return value;
                }
            }
            catch (Exception ex)
            {
            }
            try
            {
                var flag = getMetodParameterValueByType(parameter, data.TypeParams, out object value);
                if (flag)
                {
                    return value;
                }
            }
            catch (Exception ex)
            {
            }
            try
            {
                var flag = getMetodParamsCount1Value(parameter, paramsData, data.ParamsJson, out object value);
                if (flag)
                {
                    return value;
                }
            }
            catch (Exception ex)
            {
            }
            return null;
        }

        private bool getMetodParameterValueByType(ParameterInfo parameter, IList<object> typeParams, out object value)
        {
            value = null;
            if (typeParams == null)
            {
                return false;
            }
            var type = parameter.ParameterType;
            foreach (var p in typeParams)
            {
                if (p == null)
                {
                    continue;
                }
                if (type.IsAssignableFrom(p.GetType()))
                {
                    value = p;
                    return true;
                }
            }
            return false;
        }


        private bool getMetodParameterValueByName(ParameterInfo parameter, Dictionary<string, string> nameParams, out object value)
        {
            value = null;
            if (nameParams == null)
            {
                return false;
            }
            var name = parameter.Name.ToLower();
            foreach (var p in nameParams)
            {
                if (string.IsNullOrWhiteSpace(p.Key))
                {
                    continue;
                }
                if (name != p.Key.ToLower())
                {
                    continue;
                }
                if (string.IsNullOrWhiteSpace(p.Value))
                {
                    return true;
                }
                value = p.Value.ToObject(parameter.ParameterType);
                return true;
            }
            return false;
        }


        private bool getMetodParameterValue(ParameterInfo parameter, string paramJson, out object value)
        {
            value = null;
            if (string.IsNullOrWhiteSpace(paramJson))
            {
                return false;
            }
            var name = parameter.Name.ToLower();
            var jobject = paramJson.ToJObject();
            foreach (var token in jobject)
            {
                if (string.IsNullOrWhiteSpace(token.Key))
                {
                    continue;
                }
                if (name.ToLower() != token.Key.ToLower())
                {
                    continue;
                }
                if (token.Value == null)
                {
                    return true;
                }
                value = token.Value.ToObject(parameter.ParameterType);
                return true;
            }
            return false;
        }

        private bool getMetodParamsCount1Value(ParameterInfo parameter, ParamsData paramsData, string paramJson, out object value)
        {
            value = null;
            if (paramsData == null
                || paramsData.ParamsCount != 1
                || string.IsNullOrWhiteSpace(paramsData.Param1Name)
                || paramsData.Param1Name.ToLower() != parameter.Name.ToLower()
                )
            {
                return false;
            }
            value = paramJson.ToObject(parameter.ParameterType);
            return true;
        }

        private bool isActionParameter(ParameterInfo parameter)
        {
            var type = parameter.ParameterType;
            if (type == null)
            {
                return false;
            }
            return type.IsAssignableFrom(typeof(Action<object>));
        }
        private object getTypeDefaultValue(ParameterInfo parameter)
        {
            return parameter.ParameterType.DefaultValue();
        }
    }
}
