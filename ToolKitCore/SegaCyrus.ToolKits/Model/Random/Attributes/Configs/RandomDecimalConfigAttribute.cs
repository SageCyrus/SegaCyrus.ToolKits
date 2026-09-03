using System;

namespace SegaCyrus.ToolKits.Model.Random.Attributes.Configs
{
    /// <summary>
    /// 随机小数配置器
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class RandomDecimalConfigAttribute : RandomRangeConfigAttribute
    {
        /// <summary>
        /// 精度
        /// </summary>
        public int DecimalPlace { get; set; }

        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="min">最小值</param>
        /// <param name="max">最大值</param>
        /// <param name="decimalPlace">精度</param>
        public RandomDecimalConfigAttribute(double min = double.MinValue, double max = double.MaxValue,
            int decimalPlace = 5) : base(min, max)
        {
            DecimalPlace = decimalPlace;
        }
    }
}