using System;

namespace SegaCyrus.ToolKits.Model.AOP.Attributes
{
    /// <summary>
    /// AOP 忽略特性，标记在方法上表示该方法不参与 AOP 拦截
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class AOPIgnoreAttribute : Attribute
    {
    }
}