using System;

namespace SegaCyrus.ToolKits.Model.Random.Attributes.Configs
{
    /// <summary>
    /// 随机时间范围配置标记
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class RandomSetConfigAttribute : RandomConfigAttribute
    {
        /// <summary>
        /// 集合元素生成数量
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="count">元素数量</param>
        public RandomSetConfigAttribute(int count)
        {
            Count = count;
        }
    }
}