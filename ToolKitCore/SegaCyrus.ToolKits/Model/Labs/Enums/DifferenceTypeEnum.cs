namespace SegaCyrus.ToolKits.Model.Labs.Enums
{
    /// <summary>
    /// 差异类型
    /// </summary>
    public enum DifferenceTypeEnum
    {
        /// <summary>
        /// 未知
        /// </summary>
        None,

        /// <summary>
        /// 值不同
        /// </summary>
        ValueDifference,

        /// <summary>
        /// 类型不同
        /// </summary>
        TypeDifference,

        /// <summary>
        /// 源不存在
        /// </summary>
        SourceNotExist,

        /// <summary>
        /// 目标不存在
        /// </summary>
        TargetNotExist,

        /// <summary>
        /// 空相等
        /// </summary>
        EqualWithNull,

        /// <summary>
        /// 完全相等
        /// </summary>
        FullEqual
    }
}