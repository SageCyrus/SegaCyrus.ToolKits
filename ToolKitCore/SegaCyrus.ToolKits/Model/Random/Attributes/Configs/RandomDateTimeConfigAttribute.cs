using System;

namespace SegaCyrus.ToolKits.Model.Random.Attributes.Configs
{
    /// <summary>
    /// 随机时间范围配置标记
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class RandomDateTimeConfigAttribute : RandomRangeConfigAttribute
    {
        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="min">最小值</param>
        /// <param name="max">最大值</param>
        public RandomDateTimeConfigAttribute(string min = null, string max = null) :
            base(min == null ? DateTime.MinValue : DateTime.Parse(min),
                max == null ? DateTime.MaxValue : DateTime.Parse(max))
        {
        }
    }
}