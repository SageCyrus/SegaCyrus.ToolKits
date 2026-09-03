using SegaCyrus.ToolKits.Package.AOPPackage.Interface;
using System;
using System.Collections.Generic;

namespace SegaCyrus.ToolKits.Model.AOP
{
    /// <summary>
    /// AOP 代理创建选项
    /// </summary>
    public class AOPProxyOptions
    {
        /// <summary>
        /// 目标实例（接口代理时可为 null）
        /// </summary>
        public object Target { get; set; }

        /// <summary>
        /// 要代理的接口类型
        /// </summary>
        public Type InterfaceType { get; set; }

        /// <summary>
        /// 拦截器列表（优先级自动按 Order 排序）
        /// </summary>
        public List<IAOPInterceptor> Interceptors { get; set; } = new List<IAOPInterceptor>();

        /// <summary>
        /// 方法过滤器，返回 true 的方法才会被拦截
        /// </summary>
        public Func<System.Reflection.MethodInfo, bool> MethodFilter { get; set; }
    }
}
