using System.Collections.Generic;
using System.IO;
using SegaCyrus.ToolKits.Kit;
using SegaCyrus.ToolKits.Kit.File;
using SegaCyrus.ToolKits.Model.SpreadSheet.Enums;

namespace SegaCyrus.ToolKits.Model.SpreadSheet.Read
{
    /// <summary>
    /// 大文件CSV读取
    /// 超大文件读写首次读写会在此结构里面当前文件的预读属性,读取此文件建议使用同一个Args读取
    /// </summary>
    public class ReadBigCsvSpreadSheetArgs : ReadSpreadSheetArgs
    {
        /// <summary>
        /// 读取起始行
        /// </summary>
        internal int StartIndex { get; set; }

        /// <summary>
        /// 读取结束行
        /// </summary>
        internal int EndIndex { get; set; }

        /// <summary>
        /// 文件流
        /// </summary>
        internal BigFile CsvBigFileStream => (BigFile)SpreadSheetData;

        /// <summary>
        /// 拆分列
        /// </summary>
        internal List<long> RowSpData { get; set; }

        /// <summary>
        /// 大文件读取
        /// </summary>
        /// <param name="spreadSheetData">文件数据</param>
        /// <param name="hasHead">是否携带头</param>
        public ReadBigCsvSpreadSheetArgs(Stream spreadSheetData, bool hasHead)
            : base(new BigFile(AssertKit.AssertNotNull(spreadSheetData)), hasHead,
                (int)SpreadSheetTypeEnum.CSV)
        {
        }

        /// <summary>
        /// 设置读取范围
        /// </summary>
        /// <param name="startIndex">读取起始行</param>
        /// <param name="endIndex">读取结束行</param>
        /// <returns></returns>
        public ReadBigCsvSpreadSheetArgs SetReadRange(int startIndex, int endIndex)
        {
            AssertKit.AssertEQGreater(startIndex, 0, msg: "startIndex 必须大于 0");
            AssertKit.AssertEQGreater(endIndex, 0, msg: "endIndex 必须大于 0");
            AssertKit.AssertEQLesser(startIndex, endIndex, msg: "startIndex 必须小于 endIndex");
            StartIndex = startIndex;
            EndIndex = endIndex;
            return this;
        }

        /// <summary>
        /// 获取当前一共有多少行数据
        /// </summary>
        /// <returns>行数据</returns>
        public int GetRowCount() => RowSpData?.Count ?? -1;
    }
}