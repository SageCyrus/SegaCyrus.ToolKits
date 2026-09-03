using System.Collections.Generic;
using SegaCyrus.ToolKits.Model.SpreadSheet.Enums;

namespace SegaCyrus.ToolKits.Model.SpreadSheet.Read
{
    /// <summary>
    /// Excel读取参数
    /// </summary>
    public class ReadExcelSpreadSheetArgs : ReadSpreadSheetArgs
    {
        /// <summary>
        /// 如果当前行全部空白则跳过当前行
        /// </summary>
        public bool SkipEmptyRow { get; set; }
        /// <summary>
        /// 读取Sheet编号
        /// </summary>
        public HashSet<int> ReadSheetIndexs { get; set; }

        /// <summary>
        /// Excel数据
        /// </summary>
        public byte[] ExcelData
        {
            get => (byte[])SpreadSheetData;
        }

        /// <summary>
        /// Excel读取参数
        /// </summary>
        /// <param name="spreadSheetData">excel数据</param>
        /// <param name="hasHead">是否携带表头</param>
        /// <param name="skipEmptyRow">跳过空白行数据</param>
        public ReadExcelSpreadSheetArgs(byte[] spreadSheetData, bool hasHead = true, bool skipEmptyRow = true)
            : this(spreadSheetData, new HashSet<int> { 0 }, hasHead, skipEmptyRow)
        {
        }

        /// <summary>
        /// Excel读取参数
        /// </summary>
        /// <param name="spreadSheetData">excel数据</param>
        /// <param name="readHashSet">读取sheet</param>
        /// <param name="hasHead">是否携带表头</param>
        /// <param name="skipEmptyRow">跳过空白行数据</param>
        public ReadExcelSpreadSheetArgs(byte[] spreadSheetData, HashSet<int> readHashSet, bool hasHead = true, bool skipEmptyRow = true)
            : base(spreadSheetData, hasHead, (int)SpreadSheetTypeEnum.EXCEL)
        {
            ReadSheetIndexs = readHashSet;
            SkipEmptyRow = skipEmptyRow;
        }
    }
}