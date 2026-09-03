using SegaCyrus.ToolKits.Kit;
using System;

namespace SegaCyrus.ToolKits.Model.SpreadSheet.Attributes.Excel
{
    /// <summary>
    /// 
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class ExcelColumnDefineAttribute : SpreadSheetColumnDefineAttribute
    {
        /// <summary>
        /// 列宽
        /// </summary>
        public int ColumnWidth { get; set; } = -1;

        /// <summary>
        /// 自动列宽
        /// </summary>
        public bool AutoSize { get; set; }

        /// <summary>
        /// 自动换行
        /// </summary>
        public bool AutoWrapText { get; set; }

        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="propertyName">列名定义</param>
        /// <param name="order">放置位置,若冲突则随机放置</param>
        /// <param name="autoColumnSize">自动列宽</param>
        /// <param name="autoWrapText">当属性为string.empty时默认赋值 优先级大于入参配置</param>
        /// <param name="columnWidth">列宽(自动列宽失效) 值大于0时生效</param>
        /// <param name="defaultValue">当属性为string.empty时默认赋值</param>
        /// <param name="nullDefaultValue">当属性null时默认赋值</param>
        public ExcelColumnDefineAttribute(string propertyName = null,
            int order = -1,
            bool autoColumnSize = true,
            bool autoWrapText = false,
            int columnWidth = -1,
            string defaultValue = "",
            string nullDefaultValue = "") : base(propertyName, order, defaultValue, nullDefaultValue)
        {
            AssertKit.AssertFalse(autoColumnSize && autoWrapText, "自动列宽和自动换行不能同时生效");
            ColumnWidth = columnWidth;
            AutoSize = autoColumnSize;
            AutoWrapText = autoWrapText;
        }
    }
}