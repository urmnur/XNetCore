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
    public static class NullableExtend
    {
        public static T ToT<T>(this Nullable<T> t) where T : struct
        {
            if (t==null || !t.HasValue)
            {
                return default(T);
            }
            return (T)t;
        }
    }
}
