using SegaCyrus.ToolKits.Kit;
using SegaCyrus.ToolKits.Model.SpreadSheet.Enums;
using SegaCyrus.ToolKits.Model.SpreadSheet.Excel;
using SegaCyrus.ToolKits.Model.SpreadSheet.Write;
using System.Collections.Generic;

namespace SegaCyrus.ToolKits.Extension.SpreadsheetExtension.EasyWithConfig.Base
{

    /// <summary>
    /// 自动配置器
    /// </summary>
    /// <typeparam name="ChildType">子类定义</typeparam>
    /// <typeparam name="T">基本数据类型</typeparam>
    public abstract class EasyToExcelConfig<ChildType, T> where ChildType : EasyToExcelConfig<ChildType, T>
    {
        /// <summary>
        /// 数据
        /// </summary>
        protected T Data { get; set; }

        private CommonWriteExcelConfigSpreadSheetArgs Config { get; set; }

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="arg">默认配置</param>
        public EasyToExcelConfig(T data,WriteExcelConfigSpreadSheetArgs arg)
        {
            Data = data;
            Config = new CommonWriteExcelConfigSpreadSheetArgs(arg??new WriteExcelConfigSpreadSheetArgs());
        }

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="arg">默认配置</param>
        public EasyToExcelConfig(T data, DynamicDataWriteExcelConfigSpreadSheetArgs arg)
        {
            Data = data;
            Config = new CommonWriteExcelConfigSpreadSheetArgs(arg ?? new DynamicDataWriteExcelConfigSpreadSheetArgs());
        }

        /// <summary>
        /// 构造
        /// </summary>
        /// <returns>args</returns>
        protected DynamicDataWriteExcelConfigSpreadSheetArgs BuildWriteExcelConfigSpreadSheetArgs()
        {
            return Config;
        }


        /// <summary>
        /// 表头
        /// </summary>
        /// <param name="action">action</param>
        /// <returns>builder</returns>
        public ChildType SetHeadCellStyleHandle(CellStyleAction action)
        {
            Config.SetHeadCellStyleHandle(AssertKit.AssertNotNull(action));
            return (ChildType)this;
        }

        /// <summary>
        /// 表格
        /// </summary>
        /// <param name="action">action</param>
        /// <returns>builder</returns>
        public ChildType SetBodyCellStyleHandle(CellStyleAction action)
        {
            Config.SetBodyCellStyleHandle(AssertKit.AssertNotNull(action));
            return (ChildType)this;;
        }

        /// <summary>
        /// 自定义表格
        /// </summary>
        /// <param name="action">action</param>
        /// <returns>builder</returns>
        public ChildType SetFinshExcelHandle(FinishExcelAction action)
        {
            Config.SetFinshExcelHandle(AssertKit.AssertNotNull(action));
            return (ChildType)this;;
        }


        /// <summary>
        /// 是否设置表头筛选器
        /// </summary>
        public ChildType SetHeadFilter(bool value)
        {
            Config.HeadFilter(value);
            return (ChildType)this;;
        }

        /// <summary>
        /// 冻结表头
        /// </summary>
        public ChildType SetForzenHead(bool value)
        {
            Config.HeadFilter(value);
            return (ChildType)this;;
        }

        /// <summary>
        /// 冻结首列
        /// </summary>
        public ChildType SetForzenFirstColumn(bool value)
        {
            Config.SetForzenFirstColumn(value);
            return (ChildType)this;;
        }

        /// <summary>
        /// 隐藏数据源页签
        /// </summary>
        public ChildType SetHideDataSourceSheet(bool value)
        {
            Config.SetHideDataSourceSheet(value);
            return (ChildType)this;;
        }

        /// <summary>
        /// EXCEL生成格式
        /// </summary>
        public ChildType SetExcelType(ExcelTypeEnum value)
        {
            Config.SetExcelType(value);
            return (ChildType)this;;
        }

        /// <summary>
        /// 数据源生成实例
        /// </summary>
        public ChildType SetDataSourceImplList(Dictionary<string, BaseExcelDataSource> value)
        {
            Config.SetDataSourceImplList(value);
            return (ChildType)this;;
        }

        /// <summary>
        /// 大文件导出写入时生效,刷盘窗口大小
        /// </summary>
        public ChildType SetFlushDiskRowCount(int value = 1000)
        {
            Config.SetFlushDiskRowCount(value);
            return (ChildType)this;;
        }

        /// <summary>
        /// sheet页名
        /// </summary>
        /// <returns>页名</returns>
        public ChildType SetSheetName(string value)
        {
            Config.SetSheetName(value);
            return (ChildType)this;;
        }

        /// <summary>
        /// 构造数据源下拉Sheet页名
        /// </summary>
        /// <returns>页名</returns>
        public ChildType SetDropDataSourceSheetName(string value)
        {
            Config.SetDropDataSourceSheetName(value);
            return (ChildType)this;;
        }


        /// <summary>
        /// 列宽
        /// </summary>
        public ChildType SetColumnWidth(Dictionary<int, int> value)
        {
            Config.SetColumnWidth(value);
            return (ChildType)this;;
        }

        /// <summary>
        /// 行高
        /// </summary>
        public ChildType SetRowHeight(Dictionary<int, short> value)
        {
            Config.SetRowHeight(value);
            return (ChildType)this;;
        }

        /// <summary>
        /// 自动列宽
        /// </summary>
        public ChildType SetAutoColumnSize(bool enable=true)
        {
            Config.SetAutoColumnSize(enable);
            return (ChildType)this;;
        }
    }
}