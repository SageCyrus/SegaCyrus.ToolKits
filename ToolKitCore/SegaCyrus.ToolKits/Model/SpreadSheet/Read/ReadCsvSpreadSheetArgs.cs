using SegaCyrus.ToolKits.Kit;
using SegaCyrus.ToolKits.Model.SpreadSheet.Enums;

namespace SegaCyrus.ToolKits.Model.SpreadSheet.Read
{
    /// <summary>
    /// 读取CSV参数
    /// </summary>
    public class ReadCsvSpreadSheetArgs : ReadSpreadSheetArgs
    {
        /// <summary>
        /// CSV数据
        /// </summary>
        public string CsvData
        {
            get => (string)SpreadSheetData;
        }

        /// <summary>
        /// 读取CSV
        /// </summary>
        /// <param name="csvData">CSV数据</param>
        /// <param name="hasHead">是否携带表头</param>
        public ReadCsvSpreadSheetArgs(string csvData, bool hasHead = true)
            : base(AssertKit.AssertNotEmpty(csvData), hasHead, (int)SpreadSheetTypeEnum.CSV)
        {
        }
    }
}