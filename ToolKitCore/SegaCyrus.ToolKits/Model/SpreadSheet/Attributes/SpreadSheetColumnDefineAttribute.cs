using System;

namespace SegaCyrus.ToolKits.Model.SpreadSheet.Attributes
{
    /// <summary>
    /// 电子表格列定义
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class  SpreadSheetColumnDefineAttribute : Attribute
    {
        /// <summary>
        /// 列名定义
        /// </summary>
        public string PropertyName { get; set; }

        /// <summary>
        /// 放置位置
        /// </summary>
        public int Order { get; set; }

        /// <summary>
        /// 当对应的String为Empty的
        /// </summary>
        public string DefaultValue { get; set; }

        /// <summary>
        /// 当对应的String为Empty的
        /// </summary>
        public string NullDefaultValue { get; set; }

        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="propertyName">列名定义</param>
        /// <param name="order">放置位置,若冲突则随机放置</param>
        /// <param name="defaultValue">当属性为string.empty时默认赋值</param>
        /// <param name="nullDefaultValue">当属性null时默认赋值</param>
        public SpreadSheetColumnDefineAttribute(string propertyName = null, int order = -1, string defaultValue = "",
            string nullDefaultValue = "")
        {
            PropertyName = propertyName;
            Order = order;
            DefaultValue = defaultValue;
            NullDefaultValue = nullDefaultValue;
        }
    }
}