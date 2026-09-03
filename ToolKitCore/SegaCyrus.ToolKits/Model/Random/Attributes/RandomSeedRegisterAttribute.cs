using System;
using SegaCyrus.ToolKits.Kit;

namespace SegaCyrus.ToolKits.Model.Random.Attributes
{
    /// <summary>
    /// 随机种子策略注册器
    /// </summary>
    [AttributeUsage(validOn: AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public class RandomSeedRegisterAttribute : Attribute
    {
        /// <summary>
        /// 注册机
        /// </summary>
        /// <param name="type">标记的类所处理的类型</param>
        /// <param name="order">优先级</param>
        public RandomSeedRegisterAttribute(Type type, int order = int.MaxValue)
        {
            TypeDefine = AssertKit.AssertNotNull(type);
            Order = order;
        }

        /// <summary>
        /// 注册机
        /// </summary>
        /// <param name="typeName">标记的类所处理的类型</param>
        /// <param name="order">优先级</param>
        public RandomSeedRegisterAttribute(string typeName, int order = int.MaxValue)
        {
            TypeDefine = AssertKit.AssertNotNull(ReflectKit.GetTypeByFullName(typeName));
            Order = order;
        }

        /// <summary>
        /// 优先级
        /// </summary>
        public int Order { get; set; }

        /// <summary>
        /// 标记的类所处理的类型
        /// </summary>
        public Type TypeDefine { get; set; }
    }
}