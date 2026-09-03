//using System.Data;
//using SegaCyrus.ToolKits.Kit;
//using SegaCyrus.ToolKits.Model.SQL;
//using SegaCyrus.ToolKits.Model.SQL.Attributes;
//using SegaCyrus.ToolKits.Model.SQL.Enums;
//using MySql.Data.MySqlClient;

//namespace SegaCyrus.ToolKits.Package.SQLPackage.Policy
//{
//    /// <summary>
//    /// MYSQL策略
//    /// </summary>
//    [SQLRegister(SQLTypeEnum.MYSQL)]
//    public sealed class MySQLPolicy : BaseSQLPolicy
//    {
//        /// <summary>
//        /// SQL连接串构造
//        /// </summary>
//        /// <param name="sqlConnectArgs">sql配置</param>
//        /// <returns>连接串</returns>
//        public override string BuildSQLConnectStr(SQLConnectArgs sqlConnectArgs)
//        {
//            var sqlBuilder = new MySqlConnectionStringBuilder();
//            sqlBuilder.Server = sqlConnectArgs.Host;
//            sqlBuilder.Port = sqlConnectArgs.Port;
//            sqlBuilder.UserID = sqlConnectArgs.UserName;
//            sqlBuilder.Password = sqlConnectArgs.Password;
//            sqlBuilder.Database = sqlConnectArgs.DataSource;
//            sqlBuilder.DefaultCommandTimeout = sqlConnectArgs.CommandTimeout;
//            return
//                sqlBuilder
//                    .ConnectionString; //$"server={sqlConnectArgs.Host};port={sqlConnectArgs.Port};user={sqlConnectArgs.UserName};password={sqlConnectArgs.Password}; database={sqlConnectArgs.DataSource};";
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

//            using (var mysqlConnection = new MySqlConnection(BuildSQLConnectStr(sqlCommand)))
//            {
//                mysqlConnection.Open();
//                var mySqlDataAdapter = new MySqlDataAdapter(sqlCommand.ExcuteSQL, mysqlConnection);

//                var c = new DataSet();
//                mySqlDataAdapter.Fill(c, "tmp");
//                //result.InternalData = null;
//                result.TableData = c.Tables[0];

//                mysqlConnection.Close();
//            }

//            return result;
//        }
//    }
//}