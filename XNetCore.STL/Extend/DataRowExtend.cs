using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XNetCore.STL
{
    /// <summary>
    /// 数据ROW扩展
    /// </summary>
    public static class DataRowExtend
    {
        /// <summary>
        /// 获取信息
        /// </summary>
        /// <param name="row"></param>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public static string Value(this DataRow row, string fieldName)
        {
            var result = string.Empty;
            if (row == null)
            {
                return result;
            }
            if (!row.Table.Columns.Contains(fieldName))
            {
                return result;
            }

            if (row[fieldName] == null || row[fieldName] == DBNull.Value)
            {
                return result;
            }
            return row[fieldName].ToString().Trim();
        }
        


    }
}
