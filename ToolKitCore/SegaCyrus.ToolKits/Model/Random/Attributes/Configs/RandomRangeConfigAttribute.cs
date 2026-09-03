using System;
using System.ComponentModel;

namespace SegaCyrus.ToolKits.Model.Random.Attributes.Configs
{
    /// <summary>
    /// 随机种子配置
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class RandomRangeConfigAttribute : RandomConfigAttribute
    {
        /// <summary>
        /// 最大值
        /// </summary>
        public object MaxValue { get; set; }

        /// <summary>
        /// 最小值
        /// </summary>
        public object MinValue { get; set; }

        /// <summary>
        /// 构造函数 必须与所标注类型同一类型
        /// </summary> 
        /// <param name="min">最小值</param>
        /// <param name="max">最大值</param>
        [Description("必须与所标注类型同一类型")]
        public RandomRangeConfigAttribute(object min = null, object max = null)
        {
            MinValue = min ?? int.MinValue;
            MaxValue = max ?? int.MaxValue;
        }
    }
}