using System;

namespace SegaCyrus.ToolKits.Model.Expression
{
    /// <summary>
    /// Getter/Setter Model
    /// </summary>
    /// <typeparam name="InstanceType">实例类型</typeparam>
    /// <typeparam name="TargetType">操作属性/字段类型</typeparam>
    public class ExpressionGetSetModel<InstanceType, TargetType>
    {
        /// <summary>
        /// 操作属性/字段名
        /// </summary>
        public readonly string FieldName;
        /// <summary>
        /// 给对应属性/字段赋值
        /// </summary>
        public readonly Action<InstanceType, TargetType> Set;
        /// <summary>
        /// 读取对应字段/属性
        /// </summary>
        public readonly Func<InstanceType, TargetType> Get;

        /// <summary>
        /// 全参构造函数
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="settter"></param>
        /// <param name="getter"></param>
        internal ExpressionGetSetModel(string fieldName, Action<InstanceType, TargetType> settter, Func<InstanceType, TargetType> getter)
        {
            FieldName = fieldName;
            Set = settter;
            Get = getter;
        }
    }
}