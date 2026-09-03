using System;

namespace SegaCyrus.ToolKits.Model.Random.Attributes.Configs
{
    /// <summary>
    /// 随机小数配置器
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class RandomStringConfigAttribute : RandomRangeConfigAttribute
    {
        /// <summary>
        /// 是否构造复杂字符串
        /// </summary>
        public bool Complex { get; set; }

        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="min">最小值</param>
        /// <param name="max">最大值</param>
        /// <param name="isComplex">是否使用复杂参数</param>
        public RandomStringConfigAttribute(int min, int max, bool isComplex) : base(min, max)
        {
            Complex = isComplex;
        }
    }
}