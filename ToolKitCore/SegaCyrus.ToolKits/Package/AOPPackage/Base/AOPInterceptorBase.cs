using SegaCyrus.ToolKits.Model.AOP;
using SegaCyrus.ToolKits.Package.AOPPackage.Interface;

namespace SegaCyrus.ToolKits.Package.AOPPackage.Base
{
    /// <summary>
    /// AOP 拦截器抽象基类，提供默认空实现，子类按需覆写
    /// </summary>
    public abstract class AOPInterceptorBase : IAOPInterceptor
    {
        /// <summary>
        /// 默认优先级为 0
        /// </summary>
        public virtual int Order => 0;

        /// <summary>
        /// 方法执行前调用，默认返回 true（继续执行）
        /// </summary>
        public virtual bool OnBefore(AOPInterceptContext context) => true;

        /// <summary>
        /// 方法执行后调用（无论是否异常）
        /// </summary>
        public virtual void OnAfter(AOPInterceptContext context) { }

        /// <summary>
        /// 方法正常返回时调用
        /// </summary>
        public virtual void OnReturn(AOPInterceptContext context) { }

        /// <summary>
        /// 方法抛出异常时调用
        /// </summary>
        public virtual void OnException(AOPInterceptContext context) { }
    }
}
