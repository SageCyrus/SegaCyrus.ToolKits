using SegaCyrus.ToolKits.Model.SpreadSheet.Enums;

namespace SegaCyrus.ToolKits.Model.SpreadSheet.Write
{
    /// <summary>
    /// 动态数据电子表格写入配置类
    /// </summary>
    public class DynamicDataWriteExcelConfigSpreadSheetArgs : WriteExcelConfigSpreadSheetArgs
    {
        /// <summary>
        /// 自动列宽 对于强类型定义/>
        /// </summary>
        public bool AutoColumnSize { get; set; } = true;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="addHead">追加表头</param>
        /// <param name="setFilter">设置表头筛选</param>
        /// <param name="forzenHead">冻结表头</param>
        /// <param name="autoColumnSize">自动列宽</param>
        /// <param name="hideDataSource">隐藏数据源</param>
        /// <param name="type">excel类型</param>
        public DynamicDataWriteExcelConfigSpreadSheetArgs(bool addHead = true, bool setFilter = true,
            bool forzenHead = true, bool autoColumnSize = true, bool hideDataSource = true,
            ExcelTypeEnum type = ExcelTypeEnum.Xlsx)
            : base(addHead, setFilter, forzenHead, hideDataSource, type)
        {
            SetHeadFilter = setFilter;
            ExcelType = type;
            ExcelType = type;
            ForzenHead = forzenHead;
            AutoColumnSize = autoColumnSize;
            HideDataSourceSheet = hideDataSource;
        }
    }
}