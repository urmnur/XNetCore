using System;
using System.Collections.Generic;
using System.Data;

namespace XNetCore.STL
{
    /// <summary>
    /// 数据库操作类
    /// </summary>
    public class DbHelper
    {
        #region  初始化
        private DbConfig dbConfig;
        public DbHelper(DbConfig config)
        {
            this.CommandType = CommandType.Text;
            this.dbConfig = config;
        }
        #endregion

        #region DbSession
        private DbSession _dbSession;
        /// <summary>
        /// 获取数据库连接
        /// </summary>
        /// <returns></returns>
        private DbSession GetDbSession()
        {
            return new DbSession(this.dbConfig);
        }
        /// <summary>
        /// 数据库连接
        /// </summary>
        private DbSession DbSession
        {
            get
            {
                if (_dbSession == null)
                {
                    _dbSession = GetDbSession();
                }
                return _dbSession;
            }
        }
        #endregion

        #region Command
        /// <summary>
        /// 获取或设置对数据源运行的文本命令。
        /// </summary>
        public string CommandText { get; set; }
        /// <summary>
        /// 指定如何解释命令字符串。
        /// </summary>
        public CommandType CommandType { get; set; }

        #endregion

        #region Parameters
        private Dictionary<string, object> parameters = new Dictionary<string, object>();
        /// <summary>
        /// 添加数据操作参数
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void AddParameter(string key, object value)
        {
            parameters.Add(key, value);
        }
        /// <summary>
        /// 清空数据操作参数
        /// </summary>
        public void ClearParameters()
        {
            parameters.Clear();
        }

        #endregion

        #region Close
        /// <summary>
        /// 关闭数据库
        /// </summary>
        public void Close()
        {
            this.DbSession.Close();
            this._dbSession = null;
        }
        /// <summary>
        /// 关闭数据库读取
        /// </summary>
        /// <param name="reader"></param>
        private void doClose(IDataReader reader)
        {
            if (reader != null)
            {
                try
                {
                    reader.Close();
                    reader.Dispose();
                }
                catch { }
            }
            this.doClose();
        }
        /// <summary>
        /// 关闭数据库
        /// </summary>
        private void doClose()
        {
            if (this.DbSession.DbTransaction != null)
            {
                return;
            }
            this.Close();
        }
        #endregion

        #region ExecuteQuery
        private void beforeExcute()
        {
            this.DbSession.DbCommand.CommandType = this.CommandType;
            this.DbSession.DbCommand.CommandText = this.CommandText;
            foreach (var p in parameters)
            {
                var param = this.DbSession.DbCommand.CreateParameter();
                param.ParameterName = p.Key;
                var value = p.Value;
                if (value == null)
                {
                    value = DBNull.Value;
                }
                param.Value = value;
                this.DbSession.DbCommand.Parameters.Add(param);
            }
        }
        /// <summary>
        /// 执行SQL，并返回受影响的行数。
        /// </summary>
        /// <returns></returns>
        public int ExecuteNonQuery()
        {
            try
            {
                beforeExcute();
                var result = 0;
                result = this.DbSession.DbCommand.ExecuteNonQuery();
                return result;
            }
            finally
            {
                doClose();
            }
        }
        /// <summary>
        /// 执行SQL，并返回的结果集中第一行的第一列
        /// </summary>
        /// <returns></returns>
        public object ExecuteScalar()
        {
            try
            {
                beforeExcute();
                object result = null;
                result = this.DbSession.DbCommand.ExecuteScalar();
                return result;
            }
            finally
            {
                this.ClearParameters();
                doClose();
            }
        }
        /// <summary>
        /// 执行SQL，并获取数据读取
        /// </summary>
        /// <returns></returns>
        private IDataReader getDataReader()
        {
            try
            {
                beforeExcute();
                var command = this.DbSession.DbCommand;
                var result = command.ExecuteReader();
                return result;
            }
            finally
            {
                this.ClearParameters();
            }
        }
        #endregion

        #region 数据处理

        private int checkDataColumnName(string name, DataTable dTable)
        {
            var result = 0;
            foreach (DataColumn column in dTable.Columns)
            {
                if (column.ColumnName.ToLower() == name.ToLower())
                {
                    return 1;
                }
            }
            return result;
        }
        private string GetDataColumnName(string name, DataTable dTable)
        {
            if (checkDataColumnName(name, dTable) == 0)
            {
                return name;
            }
            for (var i = 1; i < 1000; i++)
            {
                var result = name + "_" + i.ToString();
                if (checkDataColumnName(result, dTable) == 0)
                {
                    return result;
                }
            }
            return name + "_" + Guid.NewGuid().ToString("D");
        }

        /// <summary>
        /// 执行SQL，并返回数据集
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        public DataSet GetDataSet(DbPage page)
        {
            var reader = getDataReader();
            var result = new DataSet();
            try
            {
                var rowIdx = (page.PageIndex - 1) * page.PageRowCount;
                if (rowIdx < 0)
                {
                    rowIdx = 0;
                }

                var rowcount = page.PageRowCount;
                var total = 0;
                var index = 0;
                var count = 0;
                do
                {
                    var dTable = new DataTable(Guid.NewGuid().ToString("D"));
                    var fieldCount = reader.FieldCount;
                    if (fieldCount == 0)
                    {
                        break;
                    }
                    for (var i = 0; i < fieldCount; i++)
                    {
                        var dc = new DataColumn();
                        dc.ColumnName = GetDataColumnName(reader.GetName(i), dTable);
                        dc.DataType = reader.GetFieldType(i);
                        dTable.Columns.Add(dc);
                    }

                    dTable.BeginLoadData();

                    while (reader.Read())
                    {
                        index++;
                        if (page.TotalRowCount == 0)
                        {
                            total++;
                        }
                        bool addRow = true;
                        if (rowIdx > 0 && rowcount > 0)
                        {
                            if (count >= rowcount)
                            {
                                break;
                            }
                            if (index <= rowIdx)
                            {
                                addRow = false;
                            }
                        }
                        if (addRow)
                        {
                            count++;
                            var row = dTable.NewRow();
                            for (var i = 0; i < fieldCount; i++)
                            {
                                if (reader.IsDBNull(i))
                                {
                                    row[i] = DBNull.Value;
                                }
                                else
                                {
                                    try
                                    {
                                        row[i] = reader.GetValue(i);
                                    }
                                    catch
                                    {
                                        row[i] = DBNull.Value;
                                    }
                                }
                            }
                            dTable.Rows.Add(row);
                        }
                    }
                    if (total > 0)
                    {
                        page.TotalRowCount = total;
                    }
                    dTable.EndLoadData();
                    result.Tables.Add(dTable);
                } while (reader.NextResult());

            }
            finally
            {
                doClose(reader);
            }
            return result;
        }
        /// <summary>
        /// 执行SQL，并返回数据集
        /// </summary>
        /// <returns></returns>
        public DataSet GetDataSet()
        {
            var page = new DbPage();
            page.TotalRowCount = 0;
            page.PageIndex = 0;
            page.PageRowCount = 0;
            return GetDataSet(page);
        }
        /// <summary>
        /// 执行SQL，并返回数据集
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        public DataTable GetDataTable(DbPage page)
        {
            var dataset = GetDataSet(page);
            if (dataset.Tables.Count > 0)
            {
                return dataset.Tables[0];
            }
            return new DataTable();
        }

        /// <summary>
        /// 执行SQL，并返回数据集
        /// </summary>
        /// <returns></returns>
        public DataTable GetDataTable()
        {
            var dataset = GetDataSet();
            if (dataset.Tables.Count > 0)
            {
                return dataset.Tables[0];
            }
            return new DataTable();
        }
        #endregion
        
    }
}
