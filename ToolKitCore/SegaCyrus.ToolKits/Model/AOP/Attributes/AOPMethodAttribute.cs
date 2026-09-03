using System;

namespace SegaCyrus.ToolKits.Model.AOP.Attributes
{
    /// <summary>
    /// AOP 方法过滤特性，标记在方法上表示该方法需要/不需要被拦截
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class AOPMethodAttribute : Attribute
    {
        /// <summary>
        /// 是否启用 AOP 拦截（默认 true）
        /// </summary>
        public bool Enable { get; set; } = true;
    }
}
