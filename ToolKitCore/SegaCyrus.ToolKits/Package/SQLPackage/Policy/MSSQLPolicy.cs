//using System.Data;
//using System.Data.SqlClient;
//using SegaCyrus.ToolKits.Kit;
//using SegaCyrus.ToolKits.Model.SQL;
//using SegaCyrus.ToolKits.Model.SQL.Attributes;
//using SegaCyrus.ToolKits.Model.SQL.Enums;

//namespace SegaCyrus.ToolKits.Package.SQLPackage.Policy
//{
//    /// <summary>
//    /// MSSQL
//    /// </summary>
//    [SQLRegister(SQLTypeEnum.MSSQL)]
//    public sealed class MSSQLPolicy : BaseSQLPolicy
//    {
//        /// <summary>
//        /// SQL连接串构造
//        /// </summary>
//        /// <param name="sqlConnectArgs">sql配置</param>
//        /// <returns>连接串</returns>
//        public override string BuildSQLConnectStr(SQLConnectArgs sqlConnectArgs)
//        {
//            var sqlBuilder = new SqlConnectionStringBuilder();
//            sqlBuilder.DataSource = $"{sqlConnectArgs.Host}";
//            sqlBuilder.UserID = sqlConnectArgs.UserName;
//            sqlBuilder.Password = sqlConnectArgs.Password;
//            sqlBuilder.InitialCatalog = sqlConnectArgs.DataSource;
//            sqlBuilder.ConnectTimeout = (int)sqlConnectArgs.CommandTimeout;
//            return sqlBuilder.ConnectionString;
//        }

//        /// <summary>
//        /// SQL执行
//        /// </summary>
//        /// <param name="sqlCommand">sql参数</param>
//        /// <returns>执行结果</returns>
//        public override SQLCommonResult ExcuteReadSQL(SQLCommonArgs sqlCommand)
//        {
//            AssertKit.AssertNotNull(sqlCommand, "args");
//            AssertKit.AssertNotEmpty(sqlCommand.DataSource, "args.DataSource");
//            AssertKit.AssertNotEmpty(sqlCommand.Host, "args.Host");
//            AssertKit.AssertNotEmpty(sqlCommand.UserName, "args.UserName");
//            AssertKit.AssertNotEmpty(sqlCommand.Password, "args.Password");
//            AssertKit.AssertNotEmpty(sqlCommand.ExcuteSQL, "args.ExcuteSQL");

//            var result = new SQLCommonResult();
//            //result.WithHead = true;

//            using (var sqlConnext = new SqlConnection(BuildSQLConnectStr(sqlCommand)))
//            {
//                sqlConnext.Open();
//                var sqlDataAdapter = new SqlDataAdapter(sqlCommand.ExcuteSQL, sqlConnext);
//                var tmp = new DataSet();
//                sqlDataAdapter.Fill(tmp, "ToolKits");
//                //result.InternalData = null;
//                result.TableData = tmp.Tables[0];
//            }

//            return result;
//        }
//    }
//}