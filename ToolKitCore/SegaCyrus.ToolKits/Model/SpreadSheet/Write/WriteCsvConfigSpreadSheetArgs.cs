using SegaCyrus.ToolKits.Model.SpreadSheet.Enums;

namespace SegaCyrus.ToolKits.Model.SpreadSheet.Write
{
    /// <summary>
    /// CSV电子表格写入类
    /// </summary>
    public class WriteCsvConfigSpreadSheetArgs : WriteConfigSpreadSheetArgs
    {
        /// <summary>
        /// CSV电子表格写入类
        /// </summary>
        /// <param name="addHead">是否添加表头</param>
        public WriteCsvConfigSpreadSheetArgs(bool addHead = true) : base(SpreadSheetTypeEnum.CSV, addHead)
        {
        }
    }
}