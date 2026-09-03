using System;
using System.Collections.Generic;
using SegaCyrus.ToolKits.Model.SpreadSheet.Enums;
using SegaCyrus.ToolKits.Model.SpreadSheet.Excel;
using NPOI.SS.UserModel;

namespace SegaCyrus.ToolKits.Model.SpreadSheet.Write
{
    /// <summary>
    /// 电子表格通用写入类
    /// </summary>
    public class WriteExcelConfigSpreadSheetArgs : WriteConfigSpreadSheetArgs
    {
        /// <summary>
        /// 是否设置表头筛选器
        /// </summary>
        public bool SetHeadFilter { get; set; }

        /// <summary>
        /// 冻结表头
        /// </summary>
        public bool ForzenHead { get; set; }

        /// <summary>
        /// 冻结首列
        /// </summary>
        public bool ForzenFirstColumn { get; set; }


        /// <summary>
        /// 隐藏数据源页签
        /// </summary>
        public bool HideDataSourceSheet { get; set; }

        /// <summary>
        /// EXCEL生成格式
        /// </summary>
        public ExcelTypeEnum ExcelType { get; set; }

        /// <summary>
        /// 数据源生成实例
        /// </summary>
        public Dictionary<string, BaseExcelDataSource> DataSourceImplList { get; set; }


        /// <summary>
        /// 大文件导出写入时生效,刷盘窗口大小
        /// </summary>
        public int FlushDiskRowCount { get; set; } = 1000;

        /// <summary>
        /// 表头样式
        /// </summary>
        /// <param name="headCellStyle">默认表头样式</param>
        /// <param name="font">默认字体</param>
        /// <returns>表头样式</returns>
        public virtual ICellStyle HeadCellStyle(ICellStyle headCellStyle, IFont font) => headCellStyle;

        /// <summary>
        /// 数据体样式
        /// </summary>
        /// <param name="bodyCellStyle">默认数据体样式</param>
        /// <param name="font">默认字体</param>
        /// <returns>数据体样式</returns>
        public virtual ICellStyle BodyCellStyle(ICellStyle bodyCellStyle, IFont font) => bodyCellStyle;

        /// <summary>
        /// 数据体样式
        /// </summary>
        /// <param name="workBook">workBook</param>
        /// <returns>workBook</returns>
        public virtual IWorkbook FinishWriteExcel(IWorkbook workBook) => workBook;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="addHead">追加表头</param>
        /// <param name="setFilter">设置表头筛选</param>
        /// <param name="forzenHead">冻结表头</param>
        /// <param name="hideDataSource">隐藏数据源</param>
        /// <param name="type">excel类型</param>
        public WriteExcelConfigSpreadSheetArgs(bool addHead = true, bool setFilter = true,
            bool forzenHead = true, bool hideDataSource = true,
            ExcelTypeEnum type = ExcelTypeEnum.Xlsx)
            : base(SpreadSheetTypeEnum.EXCEL, addHead)
        {
            //SheetSize = sheetSize;
            SetHeadFilter = setFilter;
            ExcelType = type;
            ExcelType = type;
            ForzenHead = forzenHead;
            HideDataSourceSheet = hideDataSource;
        }

        ///// <summary>
        ///// 分组聚合,根据指定Key对数据进行聚合拆分sheet
        ///// </summary>
        ///// <param name="dataItem"></param>
        ///// <returns></returns>
        //public virtual string GroupSheet(ExcelDataItem dataItem) => string.Empty;
        /// <summary>
        /// sheet页名
        /// </summary>
        /// <returns>页名</returns>
        public virtual string SheetName { get; set; } = $"ToolKit";

        /// <summary>
        /// 构造数据源下拉Sheet页名
        /// </summary>
        /// <returns>页名</returns>
        public virtual string DropDataSourceSheetName { get; set; } = "ToolKitDsSheet";

        /// <summary>
        /// 列宽
        /// </summary>
        internal Dictionary<int, int> InternalColumnWidth { get; set; }

        /// <summary>
        /// 行高
        /// </summary>
        internal Dictionary<int, short> InternalRowHeight { get; set; }

        /// <summary>
        /// 列宽
        /// </summary>
        internal Dictionary<int, int> InternalBackColumnWidth { get; set; }

        /// <summary>
        /// 行高
        /// </summary>
        internal Dictionary<int, short> InternalBackRowHeight { get; set; }


        /// <summary>
        /// 列宽
        /// </summary>
        public Dictionary<int, int> ColumnWidth
        {
            get => InternalBackColumnWidth;
            set
            {
                InternalBackColumnWidth = value;
                InternalColumnWidth = value;
            }
        }

        /// <summary>
        /// 行高
        /// </summary>
        public Dictionary<int, short> RowHeight
        {
            get => InternalBackRowHeight;
            set
            {
                InternalBackRowHeight = value;
                InternalRowHeight = value;
            }
        }
    }
}