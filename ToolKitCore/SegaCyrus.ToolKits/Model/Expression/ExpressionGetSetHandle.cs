using System;

namespace SegaCyrus.ToolKits.Model.Expression
{
    /// <summary>
    /// Getter/Setter Model
    /// </summary>
    /// <typeparam name="InstanceType">实例类型</typeparam>
    /// <typeparam name="TargetType">操作属性/字段类型</typeparam>
    public class ExpressionGetSetHandle<InstanceType, TargetType>
    {

        /// <summary>
        /// 操作属性/字段名
        /// </summary>
        public readonly string Name;
        /// <summary>
        /// 存在赋值器
        /// </summary>
        public readonly bool ExistSetHandle;
        /// <summary>
        /// 存在取值器
        /// </summary>
        public readonly bool ExistGetHandle;
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
        /// <param name="name"></param>
        /// <param name="settter"></param>
        /// <param name="getter"></param>
        /// <param name="existGet"></param>
        /// <param name="existSet"></param>
        internal ExpressionGetSetHandle(string name, Action<InstanceType, TargetType> settter, bool existSet, Func<InstanceType, TargetType> getter, bool existGet)
        {
            Name = name;
            Set = settter;
            Get = getter;
            ExistSetHandle = existSet;
            ExistGetHandle = existGet;
        }
    }
}