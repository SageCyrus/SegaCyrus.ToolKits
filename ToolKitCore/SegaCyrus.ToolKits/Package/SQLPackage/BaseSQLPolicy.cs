//using System;
//using System.Data;
//using System.Text;
//using SegaCyrus.ToolKits.Model.SQL;
//using SegaCyrus.ToolKits.Model.SQL.Enums;

//namespace SegaCyrus.ToolKits.Package.SQLPackage
//{
//    /// <summary>
//    /// SQL策略
//    /// </summary>
//    public abstract class BaseSQLPolicy
//    {
//        /// <summary>
//        /// SQL连接串构造
//        /// </summary>
//        /// <param name="sqlConnectArgs">sql配置</param>
//        /// <returns>连接串</returns>
//        public abstract string BuildSQLConnectStr(SQLConnectArgs sqlConnectArgs);

//        /// <summary>
//        /// SQL执行
//        /// </summary>
//        /// <param name="sqlCommand">sql参数</param>
//        /// <returns>执行结果</returns>
//        public abstract SQLCommonResult ExcuteReadSQL(SQLCommonArgs sqlCommand);

//        /// <summary>
//        /// 是否SQL类型 （Unstable）
//        /// </summary>
//        /// <param name="type"></param>
//        /// <returns></returns>
//        [Obsolete("随时可能被重构修改")]
//        public bool IsSQLType(Type type)
//        {
//            return type.GUID == typeof(int).GUID
//                   || type.GUID == typeof(long).GUID
//                   || type.GUID == typeof(short).GUID
//                   || type.GUID == typeof(decimal).GUID
//                   || type.GUID == typeof(double).GUID
//                   || type.GUID == typeof(float).GUID
//                   || type.GUID == typeof(DateTime).GUID
//                   || type.GUID == typeof(string).GUID
//                   || type.GUID == typeof(Guid).GUID
//                   || type.GUID == typeof(uint).GUID
//                   || type.GUID == typeof(ulong).GUID
//                   || type.GUID == typeof(double).GUID
//                   || type.GUID == typeof(byte[]).GUID
//                   || type.GUID == typeof(char).GUID;
//        }

//        /// <summary>
//        /// DataTable转SQL
//        /// </summary>
//        /// <param name="dataTable">DataTable</param>
//        /// <param name="sqlMode">操作方式</param>
//        /// <returns></returns>
//        public virtual string ToSQL(DataTable dataTable, SQLModelEnum sqlMode)
//        {
//            Func<Type, bool> isDigital = type =>
//                type.GUID == typeof(int).GUID
//                || type.GUID == typeof(long).GUID
//                || type.GUID == typeof(short).GUID
//                || type.GUID == typeof(decimal).GUID
//                || type.GUID == typeof(double).GUID
//                || type.GUID == typeof(float).GUID
//                || type.GUID == typeof(uint).GUID
//                || type.GUID == typeof(ulong).GUID
//                || type.GUID == typeof(double).GUID;

//            var result = new StringBuilder();
//            if (sqlMode == SQLModelEnum.Insert)
//            {
//                for (var i = 0; i < dataTable.Rows.Count; i++)
//                {
//                    result.AppendFormat("INSERT INTO {0}(", dataTable.TableName);

//                    //拼前半段INSERT
//                    for (var j = 0; j < dataTable.Columns.Count; j++)
//                    {
//                        if (j == dataTable.Columns.Count - 1)
//                            result.AppendFormat("{0})", dataTable.Columns[j].Caption);
//                        else
//                            result.AppendFormat("{0},", dataTable.Columns[j].Caption);
//                    }

//                    //拼后半段VALUES
//                    result.Append(" VALUES(");
//                    for (var j = 0; j < dataTable.Columns.Count; j++)
//                    {
//                        if (j == dataTable.Columns.Count - 1)
//                        {
//                            if (isDigital(dataTable.Rows[i][j].GetType()))
//                                result.AppendFormat("{0})", dataTable.Rows[i][j]);
//                            else
//                                result.AppendFormat("'{0}')", dataTable.Rows[i][j]);
//                        }
//                        else if (isDigital(dataTable.Rows[i][j].GetType()))
//                            result.AppendFormat("{0},", dataTable.Rows[i][j]);
//                        else
//                            result.AppendFormat("'{0}',", dataTable.Rows[i][j]);
//                    }

//                    result.Append(";\n");
//                }
//            }

//            return result.ToString();
//        }
//    }
//}