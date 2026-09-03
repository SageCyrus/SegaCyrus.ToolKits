using System;
using SegaCyrus.ToolKits.Model.SpreadSheet.Enums;

namespace SegaCyrus.ToolKits.Model.SpreadSheet.Attributes
{
    /// <summary>
    /// 电子表格注册器
    /// </summary>
    public class SpreadSheetRegisterAttribute : Attribute
    {
        /// <summary>
        /// 所处理的类型
        /// </summary>
        public int TargetType { get; set; }

        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="targetType">标记策略所处理的类型</param>
        public SpreadSheetRegisterAttribute(SpreadSheetTypeEnum targetType)
        {
            TargetType = (int)targetType;
        }

        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="targetType">标记策略所处理的类型</param>
        public SpreadSheetRegisterAttribute(int targetType)
        {
            TargetType = targetType;
        }
    }
}