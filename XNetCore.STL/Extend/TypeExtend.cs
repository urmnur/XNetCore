using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace XNetCore.STL
{
    /// <summary>
    /// 字符串扩展
    /// </summary>
    public static class TypeExtend
    {
        public static object DefaultValue(this Type type)
        {
            if (type == null)
            {
                return null;
            }
            if (type == typeof(void))
            {
                return null;
            }
            return type.IsValueType ? Activator.CreateInstance(type) : null;
        }

        public static string FullName(this Type type)
        {
            if (type == null)
            {
                return string.Empty;
            }
            var result = string.Empty;
            result = type.AssemblyQualifiedName;
            var idx = result.IndexOf(',', 0);
            if (idx < 0)
            {
                return result;
            }
            idx = result.IndexOf(',', idx + 1);
            if (idx < 0)
            {
                return result;
            }
            result = result.Substring(0, idx);
            return result;
        }

        public static Type ToType(this string typeName)
        {
            try
            {
                var type = Type.GetType(typeName, false, true);
                return type;
            }
            catch
            {

            }
            return null;
        }
        public static object Instance(this Type type)
        {
            if (type == null)
            {
                return null;
            }
            return Activator.CreateInstance(type, null);
        }
        public static object Instance(this Type type, object[] args)
        {
            if (type == null)
            {
                return null;
            }
            return Activator.CreateInstance(type, args);
        }

        private static bool IsNullableType(Type t)
        {
            if (t == null)
            {
                return false;
            }
            return (t.BaseType.FullName == "System.ValueType" && t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Nullable<>));
        }
        public static bool IsSimpleType(this Type type)
        {
            if (type == null)
            {
                return false;
            }
            if (type.IsValueType)
            {
                return true;
            }
            if (type.IsPrimitive)
            {
                return true;
            }
            var tName = type.Name.ToString().ToLower();
            if (IsNullableType(type))
            {
                tName = Nullable.GetUnderlyingType(type).Name.ToString().ToLower();
            }
            if (tName == "int16" ||
                 tName == "int32" ||
                 tName == "int64" ||
                 tName == "string" ||
                 tName == "datetime" ||
                 tName == "boolean" ||
                 tName == "char" ||
                 tName == "double")
            {
                return true;
            }
            return false;
        }


        public static PropertyInfo Property(this Type type,string pName)
        {
            var result = type.GetProperty(pName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            return result;
        }
    }
}
