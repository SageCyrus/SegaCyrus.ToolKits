using System;
using SegaCyrus.ToolKits.Model.SQL.Enums;

namespace SegaCyrus.ToolKits.Model.SQL.Attributes
{
    /// <summary>
    /// SQL注册器
    /// </summary>
    public class SQLRegisterAttribute : Attribute
    {
        /// <summary>
        /// 标记策略处理的类型
        /// </summary>
        public SQLTypeEnum TargetType { get; set; }

        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="targetType">标记策略处理的类型</param>
        public SQLRegisterAttribute(SQLTypeEnum targetType)
        {
            TargetType = targetType;
        }
    }
}