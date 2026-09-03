using SegaCyrus.ToolKits.Package.AOPPackage.Interface;
using System;

namespace SegaCyrus.ToolKits.Model.AOP.Attributes
{
    /// <summary>
    /// AOP 切面标记特性，标记在需要被拦截的类或接口上
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = true, Inherited = true)]
    public class AOPAttribute : Attribute
    {
        /// <summary>
        /// 拦截器类型，必须实现 IAOPInterceptor
        /// </summary>
        public Type InterceptorType { get; }

        /// <summary>
        /// 指定需要拦截的 AOP 切面拦截器
        /// </summary>
        /// <param name="interceptorType">拦截器类型，必须实现 IAOPInterceptor</param>
        public AOPAttribute(Type interceptorType)
        {
            if (!typeof(IAOPInterceptor).IsAssignableFrom(interceptorType))
                throw new ArgumentException($"类型 {interceptorType.FullName} 必须实现 IAOPInterceptor 接口", nameof(interceptorType));
            InterceptorType = interceptorType;
        }
    }

}
