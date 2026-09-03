//using SegaCyrus.ToolKits.Kit;
//using SegaCyrus.ToolKits.Model.SpreadSheet.Enums;
//using SegaCyrus.ToolKits.Model.SQL.Enums;

//namespace SegaCyrus.ToolKits.Model.SpreadSheet.Write
//{
//    /// <summary>
//    /// SQL电子表格写
//    /// </summary>
//    public class WriteSqlConfigSpreadSheetArgs : WriteConfigSpreadSheetArgs
//    {
//        /// <summary>
//        /// 构造方式
//        /// </summary>
//        public SQLModelEnum SQLModel { get; set; }

//        /// <summary>
//        /// 表名
//        /// </summary>
//        public string TableName { get; set; }

//        /// <summary>
//        /// 目标SQL类型
//        /// </summary>
//        public SQLTypeEnum SQLType { get; set; }

//        /// <summary>
//        /// SQL电子表格写入类构造方式
//        /// </summary>
//        /// <param name="sqlType">sql类型</param>
//        /// <param name="tableName">sql表名</param>
//        /// <param name="buildModel">构造方式</param>
//        public WriteSqlConfigSpreadSheetArgs(SQLTypeEnum sqlType, string tableName,
//            SQLModelEnum buildModel = SQLModelEnum.Insert) : base(
//            SpreadSheetTypeEnum.SQL, true)
//        {
//            SQLType = sqlType;
//            SQLModel = buildModel;
//            TableName = AssertKit.AssertNotNull(tableName, nameof(tableName));
//        }
//    }
//}