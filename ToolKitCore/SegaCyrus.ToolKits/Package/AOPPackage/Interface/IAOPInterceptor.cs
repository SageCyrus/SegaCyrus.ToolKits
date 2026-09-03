using SegaCyrus.ToolKits.Model.AOP;

namespace SegaCyrus.ToolKits.Package.AOPPackage.Interface
{
    /// <summary>
    /// AOP 拦截器接口，所有切面逻辑需实现此接口
    /// </summary>
    public interface IAOPInterceptor
    {
        /// <summary>
        /// 拦截器优先级，数值越小优先级越高（先执行）
        /// </summary>
        int Order { get; }

        /// <summary>
        /// 方法执行前调用
        /// </summary>
        /// <param name="context">拦截上下文</param>
        /// <returns>返回 true 继续执行后续拦截器和目标方法；返回 false 则短路跳过</returns>
        bool OnBefore(AOPInterceptContext context);

        /// <summary>
        /// 方法执行后调用（无论是否发生异常都会执行，类似 finally）
        /// </summary>
        /// <param name="context">拦截上下文</param>
        void OnAfter(AOPInterceptContext context);

        /// <summary>
        /// 方法正常返回时调用
        /// </summary>
        /// <param name="context">拦截上下文</param>
        void OnReturn(AOPInterceptContext context);

        /// <summary>
        /// 方法抛出异常时调用
        /// </summary>
        /// <param name="context">拦截上下文</param>
        void OnException(AOPInterceptContext context);
    }

}
