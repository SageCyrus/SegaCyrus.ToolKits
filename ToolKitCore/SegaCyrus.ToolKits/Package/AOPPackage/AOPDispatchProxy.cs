#if NET6_0_OR_GREATER
using System;
using System.Collections.Generic;
using System.Reflection;
using SegaCyrus.ToolKits.Model.AOP;
using SegaCyrus.ToolKits.Model.AOP.Attributes;
using SegaCyrus.ToolKits.Package.AOPPackage.Interface;

namespace SegaCyrus.ToolKits.Kit.AOP
{
    /// <summary>
    /// 基于 DispatchProxy 的 AOP 代理
    /// </summary>
    internal class AOPDispatchProxy<T> : System.Reflection.DispatchProxy where T : class
    {
        private T _target;
        private Type _interfaceType;
        private List<IAOPInterceptor> _interceptors;
        private Func<MethodInfo, bool> _methodFilter;

        public static T Create(T target, Type interfaceType, List<IAOPInterceptor> interceptors, Func<MethodInfo, bool> methodFilter)
        {
            var proxy = Create<T, AOPDispatchProxy<T>>() as AOPDispatchProxy<T>;
            proxy._target = target;
            proxy._interfaceType = interfaceType;
            proxy._interceptors = interceptors ?? new List<IAOPInterceptor>();
            proxy._methodFilter = methodFilter;
            return proxy as T;
        }

        protected override object Invoke(MethodInfo targetMethod, object[] args)
        {
            // 判断是否需要拦截
            if (!ShouldIntercept(targetMethod))
            {
                return targetMethod.Invoke(_target, args);
            }

            var context = new AOPInterceptContext
            {
                Target = _target,
                Method = targetMethod,
                Arguments = args
            };

            try
            {
                // Before 拦截
                bool shouldContinue = InvokeBefore(context);
                if (!shouldContinue)
                {
                    // 短路，返回默认值
                    return targetMethod.ReturnType.IsValueType
                        ? Activator.CreateInstance(targetMethod.ReturnType)
                        : null;
                }

                // 执行目标方法
                var result = targetMethod.Invoke(_target, args);
                context.ReturnValue = result;

                // After（正常返回）
                InvokeAfter(context);
                InvokeReturn(context);

                return result;
            }
            catch (TargetInvocationException tex)
            {
                context.Exception = tex.InnerException ?? tex;
                InvokeAfter(context);
                InvokeException(context);
                throw; // 重新抛出异常
            }
            catch (Exception ex)
            {
                context.Exception = ex;
                InvokeAfter(context);
                InvokeException(context);
                throw;
            }
        }

        private bool ShouldIntercept(MethodInfo method)
        {
            if (method.GetCustomAttribute<AOPIgnoreAttribute>() != null)
                return false;

            var methodAttr = method.GetCustomAttribute<AOPMethodAttribute>();
            if (methodAttr != null && !methodAttr.Enable)
                return false;

            if (_methodFilter != null && !_methodFilter(method))
                return false;

            return true;
        }

        private bool InvokeBefore(AOPInterceptContext context)
        {
            foreach (var interceptor in _interceptors)
            {
                if (!interceptor.OnBefore(context))
                    return false;
            }
            return true;
        }

        private void InvokeAfter(AOPInterceptContext context)
        {
            foreach (var interceptor in _interceptors)
            {
                interceptor.OnAfter(context);
            }
        }

        private void InvokeReturn(AOPInterceptContext context)
        {
            foreach (var interceptor in _interceptors)
            {
                interceptor.OnReturn(context);
            }
        }

        private void InvokeException(AOPInterceptContext context)
        {
            foreach (var interceptor in _interceptors)
            {
                interceptor.OnException(context);
            }
        }
    }
}
#endif
