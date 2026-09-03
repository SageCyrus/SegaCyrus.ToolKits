using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using SegaCyrus.ToolKits.Kit.AOP;
using SegaCyrus.ToolKits.Model.AOP;
using SegaCyrus.ToolKits.Model.AOP.Attributes;
using SegaCyrus.ToolKits.Package.AOPPackage.Interface;

namespace SegaCyrus.ToolKits.Kit
{
    /// <summary>
    /// AOP 面向切面编程工具包
    /// <para>支持方法拦截、前置/后置/环绕切面、基于Attribute的切面定义、动态代理</para>
    /// </summary>
    public class AOPKit
    {
        #region 代理创建

        /// <summary>
        /// 为指定接口类型创建 AOP 动态代理
        /// </summary>
        /// <typeparam name="TInterface">接口类型</typeparam>
        /// <param name="target">目标实例</param>
        /// <param name="interceptors">拦截器列表</param>
        /// <param name="methodFilter">方法过滤器（可选），返回 true 的方法才会被拦截</param>
        /// <returns>代理实例</returns>
        public static TInterface CreateProxy<TInterface>(TInterface target,
            List<IAOPInterceptor> interceptors = null,
            Func<MethodInfo, bool> methodFilter = null) where TInterface : class
        {
            AssertKit.AssertNotNull(target, nameof(target));
            var interfaceType = typeof(TInterface);
            AssertKit.AssertTrue(interfaceType.IsInterface,
                msg: $"TInterface 必须为接口类型，当前类型: {interfaceType.FullName}");

            return CreateProxyInternal(target, interfaceType, interceptors, methodFilter);
        }

        /// <summary>
        /// 为指定实例创建 AOP 动态代理（自动扫描 AOPAttribute 特性）
        /// </summary>
        /// <typeparam name="TInterface">接口类型</typeparam>
        /// <param name="target">目标实例</param>
        /// <returns>代理实例</returns>
        public static TInterface CreateProxyByAttribute<TInterface>(TInterface target) where TInterface : class
        {
            AssertKit.AssertNotNull(target, nameof(target));
            var interfaceType = typeof(TInterface);

            var interceptors = GetInterceptorsFromAttribute(interfaceType);
            return CreateProxyInternal(target, interfaceType, interceptors, null);
        }

        /// <summary>
        /// 为指定实例创建 AOP 动态代理（完整选项）
        /// </summary>
        /// <typeparam name="TInterface">接口类型</typeparam>
        /// <param name="options">代理选项</param>
        /// <returns>代理实例</returns>
        public static TInterface CreateProxy<TInterface>(AOPProxyOptions options) where TInterface : class
        {
            AssertKit.AssertNotNull(options, nameof(options));
            var interfaceType = options.InterfaceType ?? typeof(TInterface);
            AssertKit.AssertTrue(interfaceType.IsInterface,
                msg: $"代理类型必须为接口类型，当前类型: {interfaceType.FullName}");

            var target = options.Target as TInterface;
            return CreateProxyInternal(target, interfaceType, options.Interceptors, options.MethodFilter);
        }

        #endregion

        #region 内部代理创建

        private static TInterface CreateProxyInternal<TInterface>(TInterface target,
            Type interfaceType, List<IAOPInterceptor> interceptors,
            Func<MethodInfo, bool> methodFilter) where TInterface : class
        {
            // 排序拦截器（按 Order 升序）
            var sortedInterceptors = (interceptors ?? new List<IAOPInterceptor>())
                .OrderBy(i => i.Order)
                .ToList();

#if NET8_0_OR_GREATER
            return AOPDispatchProxy<TInterface>.Create(target, interfaceType, sortedInterceptors, methodFilter);
#else
            var realProxy = new AOPRealProxy<TInterface>(target, interfaceType, sortedInterceptors, methodFilter);
            return realProxy.GetTransparentProxy() as TInterface;
#endif
        }

        #endregion

        #region 拦截器扫描

        /// <summary>
        /// 从类型的 AOPAttribute 特性中获取拦截器列表
        /// </summary>
        /// <param name="type">目标类型</param>
        /// <returns>拦截器列表（已按 Order 排序）</returns>
        public static List<IAOPInterceptor> GetInterceptorsFromAttribute(Type type)
        {
            AssertKit.AssertNotNull(type, nameof(type));

            var attributes = type.GetCustomAttributes<AOPAttribute>(true);
            var interceptors = new List<IAOPInterceptor>();

            foreach (var attr in attributes)
            {
                if (attr.InterceptorType != null)
                {
                    var instance = Activator.CreateInstance(attr.InterceptorType) as IAOPInterceptor;
                    if (instance != null)
                        interceptors.Add(instance);
                }
            }

            return interceptors.OrderBy(i => i.Order).ToList();
        }

        /// <summary>
        /// 从类型的 AOPAttribute 特性中获取拦截器列表
        /// </summary>
        /// <typeparam name="T">目标类型</typeparam>
        /// <returns>拦截器列表（已按 Order 排序）</returns>
        public static List<IAOPInterceptor> GetInterceptorsFromAttribute<T>()
        {
            return GetInterceptorsFromAttribute(typeof(T));
        }

        #endregion

        #region 简便方法：直接执行带切面的方法

        /// <summary>
        /// 使用环绕切面执行指定委托
        /// </summary>
        /// <param name="action">待执行的委托</param>
        /// <param name="interceptors">拦截器列表</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ExecuteWithAOP(Action action, params IAOPInterceptor[] interceptors)
        {
            var context = new AOPInterceptContext();
            var sorted = (interceptors ?? new IAOPInterceptor[0]).OrderBy(i => i.Order).ToList();

            try
            {
                if (!InvokeBefore(sorted, context))
                    return;

                action();
                InvokeAfter(sorted, context);
                InvokeReturn(sorted, context);
            }
            catch (Exception ex)
            {
                context.Exception = ex;
                InvokeAfter(sorted, context);
                InvokeException(sorted, context);
                throw;
            }
        }

        /// <summary>
        /// 使用环绕切面执行指定带返回值的委托
        /// </summary>
        /// <typeparam name="T">返回类型</typeparam>
        /// <param name="func">待执行的委托</param>
        /// <param name="interceptors">拦截器列表</param>
        /// <returns>执行结果</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T ExecuteWithAOP<T>(Func<T> func, params IAOPInterceptor[] interceptors)
        {
            var context = new AOPInterceptContext();
            var sorted = (interceptors ?? new IAOPInterceptor[0]).OrderBy(i => i.Order).ToList();

            try
            {
                if (!InvokeBefore(sorted, context))
                    return default(T);

                var result = func();
                context.ReturnValue = result;
                InvokeAfter(sorted, context);
                InvokeReturn(sorted, context);

                return result;
            }
            catch (Exception ex)
            {
                context.Exception = ex;
                InvokeAfter(sorted, context);
                InvokeException(sorted, context);
                throw;
            }
        }

        #endregion

        #region 拦截器调用辅助

        private static bool InvokeBefore(List<IAOPInterceptor> interceptors, AOPInterceptContext context)
        {
            foreach (var interceptor in interceptors)
            {
                if (!interceptor.OnBefore(context))
                    return false;
            }
            return true;
        }

        private static void InvokeAfter(List<IAOPInterceptor> interceptors, AOPInterceptContext context)
        {
            foreach (var interceptor in interceptors)
            {
                interceptor.OnAfter(context);
            }
        }

        private static void InvokeReturn(List<IAOPInterceptor> interceptors, AOPInterceptContext context)
        {
            foreach (var interceptor in interceptors)
            {
                interceptor.OnReturn(context);
            }
        }

        private static void InvokeException(List<IAOPInterceptor> interceptors, AOPInterceptContext context)
        {
            foreach (var interceptor in interceptors)
            {
                interceptor.OnException(context);
            }
        }

        #endregion
    }
}
