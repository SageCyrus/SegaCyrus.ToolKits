using System;
using System.Reflection;

namespace SegaCyrus.ToolKits.Model.AOP
{
    /// <summary>
    /// AOP 拦截上下文，包含当前调用的完整信息
    /// </summary>
    public class AOPInterceptContext
    {
        /// <summary>
        /// 被代理的目标实例（接口代理时为 null）
        /// </summary>
        public object Target { get; set; }

        /// <summary>
        /// 被调用的方法信息
        /// </summary>
        public MethodInfo Method { get; set; }

        /// <summary>
        /// 调用参数
        /// </summary>
        public object[] Arguments { get; set; }

        /// <summary>
        /// 方法执行后的返回值（仅在 After 和 OnReturn 阶段有效）
        /// </summary>
        public object ReturnValue { get; set; }

        /// <summary>
        /// 方法执行时抛出的异常（仅在 OnException 阶段有效）
        /// </summary>
        public Exception Exception { get; set; }

        /// <summary>
        /// 用户自定义状态数据，可在拦截器之间传递
        /// </summary>
        public object State { get; set; }
    }
}
