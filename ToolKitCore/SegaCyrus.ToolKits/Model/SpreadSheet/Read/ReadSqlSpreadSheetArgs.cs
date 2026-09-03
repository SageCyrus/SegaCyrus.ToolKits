//using SegaCyrus.ToolKits.Kit;
//using SegaCyrus.ToolKits.Model.SpreadSheet.Enums;
//using SegaCyrus.ToolKits.Model.SQL;

//namespace SegaCyrus.ToolKits.Model.SpreadSheet.Read
//{
//    /// <summary>
//    /// Sql读取参数
//    /// </summary>
//    public class ReadSqlSpreadSheetArgs : ReadSpreadSheetArgs
//    {
//        /// <summary>
//        /// 构造函数
//        /// </summary>
//        /// <param name="sqlCommand">读取参数</param>
//        public ReadSqlSpreadSheetArgs(SQLCommonArgs sqlCommand)
//            : base(AssertKit.AssertNotNull(sqlCommand), true, (int)SpreadSheetTypeEnum.SQL)
//        {
//        }

//        /// <summary>
//        /// 读取参数
//        /// </summary>
//        public SQLCommonArgs SQLCommand
//        {
//            get => (SQLCommonArgs)SpreadSheetData;
//        }
//    }
//}