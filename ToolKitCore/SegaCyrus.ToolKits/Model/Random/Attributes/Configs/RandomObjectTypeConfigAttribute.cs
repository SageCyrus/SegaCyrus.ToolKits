using System;

namespace SegaCyrus.ToolKits.Model.Random.Attributes.Configs
{
    /// <summary>
    /// object随机模型指定配置器
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class RandomObjectTypeConfigAttribute : RandomRangeConfigAttribute
    {
        /// <summary>
        /// 随机模型构造器
        /// </summary>
        /// <param name="types"></param>
        public RandomObjectTypeConfigAttribute(params Type[] types)
        {
            MinValue = types;
        }
    }
}
