using SegaCyrus.ToolKits.Kit;
using SegaCyrus.ToolKits.Model.SpreadSheet.Enums;
using SegaCyrus.ToolKits.Model.SpreadSheet.Excel;
using NPOI.SS.UserModel;
using System.Collections.Generic;

namespace SegaCyrus.ToolKits.Model.SpreadSheet.Write
{
    /// <summary>
    /// 样式函数
    /// </summary>
    /// <param name="cellStyle">cellStyle</param>
    /// <param name="font">font</param>
    /// <returns>new style</returns>
    public delegate ICellStyle CellStyleAction(ICellStyle cellStyle, IFont font);
    /// <summary>
    /// workbook函数
    /// </summary>
    /// <param name="workBook">workBook</param>
    /// <returns>workBook</returns>
    public delegate IWorkbook FinishExcelAction(IWorkbook workBook);

    internal class CommonWriteExcelConfigSpreadSheetArgs : DynamicDataWriteExcelConfigSpreadSheetArgs
    {
        private CellStyleAction HeadCellStyleHandle { get; set; }
        private CellStyleAction BodyCellStyleHandle { get; set; }
        private FinishExcelAction FinshExcelHandle { get; set; }

        internal CommonWriteExcelConfigSpreadSheetArgs SetHeadCellStyleHandle(CellStyleAction action)
        {
            HeadCellStyleHandle = action;
            return this;
        }

        internal CommonWriteExcelConfigSpreadSheetArgs SetBodyCellStyleHandle(CellStyleAction action)
        {
            BodyCellStyleHandle = action;
            return this;
        }

        internal CommonWriteExcelConfigSpreadSheetArgs SetFinshExcelHandle(FinishExcelAction action)
        {
            FinshExcelHandle = action;
            return this;
        }

        /// <summary>
        /// 是否设置表头筛选器
        /// </summary>
        public CommonWriteExcelConfigSpreadSheetArgs HeadFilter(bool value)
        {
            SetHeadFilter = value;
            return this;
        }

        /// <summary>
        /// 冻结表头
        /// </summary>
        public CommonWriteExcelConfigSpreadSheetArgs SetForzenHead(bool value)
        {
            ForzenHead = value;
            return this;
        }

        /// <summary>
        /// 冻结首列
        /// </summary>
        public CommonWriteExcelConfigSpreadSheetArgs SetForzenFirstColumn(bool value)
        {
            ForzenHead = value;
            return this;
        }

        /// <summary>
        /// 隐藏数据源页签
        /// </summary>
        public CommonWriteExcelConfigSpreadSheetArgs SetHideDataSourceSheet(bool value)
        {
            HideDataSourceSheet = value;
            return this;
        }

        /// <summary>
        /// EXCEL生成格式
        /// </summary>
        public CommonWriteExcelConfigSpreadSheetArgs SetExcelType(ExcelTypeEnum value)
        {
            ExcelType = value;
            return this;
        }

        /// <summary>
        /// 数据源生成实例
        /// </summary>
        public CommonWriteExcelConfigSpreadSheetArgs SetDataSourceImplList(Dictionary<string, BaseExcelDataSource> value)
        {
            DataSourceImplList = value;
            return this;
        }

        /// <summary>
        /// 大文件导出写入时生效,刷盘窗口大小
        /// </summary>
        public CommonWriteExcelConfigSpreadSheetArgs SetFlushDiskRowCount(int value = 1000)
        {
            FlushDiskRowCount = AssertKit.AssertPositive(FlushDiskRowCount, nameof(FlushDiskRowCount));
            return this;
        }

        /// <summary>
        /// sheet页名
        /// </summary>
        /// <returns>页名</returns>
        public CommonWriteExcelConfigSpreadSheetArgs SetSheetName(string value)
        {
            SheetName = AssertKit.AssertNotEmpty(value, nameof(SheetName));
            return this;
        }

        /// <summary>
        /// 构造数据源下拉Sheet页名
        /// </summary>
        /// <returns>页名</returns>
        public CommonWriteExcelConfigSpreadSheetArgs SetDropDataSourceSheetName(string value)
        {
            DropDataSourceSheetName = AssertKit.AssertNotEmpty(value, nameof(DropDataSourceSheetName));
            return this;
        }

        public CommonWriteExcelConfigSpreadSheetArgs SetAutoColumnSize(bool enable=true)
        {
            AutoColumnSize = enable;
            return this;
        }

        /// <summary>
        /// 列宽
        /// </summary>
        public CommonWriteExcelConfigSpreadSheetArgs SetColumnWidth(Dictionary<int, int> value)
        {
            ColumnWidth = value;
            return this;
        }

        /// <summary>
        /// 行高
        /// </summary>
        public CommonWriteExcelConfigSpreadSheetArgs SetRowHeight(Dictionary<int, short> value)
        {
            RowHeight = value;
            return this;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="headCellStyle">默认表头样式</param>
        /// <param name="font">默认字体</param>
        /// <returns>表头样式</returns>
        public override ICellStyle HeadCellStyle(ICellStyle headCellStyle, IFont font)
            => HeadCellStyleHandle == null ? base.HeadCellStyle(headCellStyle, font) : HeadCellStyleHandle(headCellStyle, font);

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="bodyCellStyle">默认数据体样式</param>
        /// <param name="font">默认字体</param>
        /// <returns>数据体样式</returns>
        public override ICellStyle BodyCellStyle(ICellStyle bodyCellStyle, IFont font)
            => BodyCellStyleHandle == null ? base.BodyCellStyle(bodyCellStyle, font) : BodyCellStyleHandle(bodyCellStyle, font);

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="workBook">workBook</param>
        /// <returns>workBook</returns>
        public override IWorkbook FinishWriteExcel(IWorkbook workBook)
            => FinshExcelHandle == null ? base.FinishWriteExcel(workBook) : FinshExcelHandle(workBook);

        internal CommonWriteExcelConfigSpreadSheetArgs(WriteExcelConfigSpreadSheetArgs arg)
            : base(arg.AddHead, arg.SetHeadFilter, arg.ForzenHead, false, arg.HideDataSourceSheet, arg.ExcelType)
        {
        }
        internal CommonWriteExcelConfigSpreadSheetArgs(DynamicDataWriteExcelConfigSpreadSheetArgs arg)
            : base(arg.AddHead, arg.SetHeadFilter, arg.ForzenHead, arg.AutoColumnSize, arg.HideDataSourceSheet, arg.ExcelType)
        {
        }
    }
}