#if !NET6_0_OR_GREATER
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Remoting.Proxies;
using SegaCyrus.ToolKits.Model.AOP;
using SegaCyrus.ToolKits.Model.AOP.Attributes;
using SegaCyrus.ToolKits.Package.AOPPackage.Interface;

namespace SegaCyrus.ToolKits.Kit.AOP
{
    /// <summary>
    /// 基于 RealProxy 的 AOP 代理（.NET Framework 4.x 专用）
    /// </summary>
    internal class AOPRealProxy<T> : RealProxy where T : class
    {
        private readonly T _target;
        private readonly Type _interfaceType;
        private readonly List<IAOPInterceptor> _interceptors;
        private readonly Func<MethodInfo, bool> _methodFilter;

        public AOPRealProxy(T target, Type interfaceType, List<IAOPInterceptor> interceptors, Func<MethodInfo, bool> methodFilter)
            : base(typeof(T))
        {
            _target = target;
            _interfaceType = interfaceType;
            _interceptors = interceptors ?? new List<IAOPInterceptor>();
            _methodFilter = methodFilter;
        }

        public override IMessage Invoke(IMessage msg)
        {
            var callMsg = msg as IMethodCallMessage;
            if (callMsg == null)
                return null;

            var method = (MethodInfo)callMsg.MethodBase;
            var args = callMsg.Args;

            // 判断是否需要拦截
            if (!ShouldIntercept(method))
            {
                var result = method.Invoke(_target, args);
                return new ReturnMessage(result, null, 0, callMsg.LogicalCallContext, callMsg);
            }

            var context = new AOPInterceptContext
            {
                Target = _target,
                Method = method,
                Arguments = args
            };

            try
            {
                // Before 拦截
                bool shouldContinue = InvokeBefore(context);
                if (!shouldContinue)
                {
                    // 短路，返回默认值
                    var defaultResult = method.ReturnType.IsValueType
                        ? Activator.CreateInstance(method.ReturnType)
                        : null;
                    return new ReturnMessage(defaultResult, null, 0, callMsg.LogicalCallContext, callMsg);
                }

                // 执行目标方法
                var invokeResult = method.Invoke(_target, args);
                context.ReturnValue = invokeResult;

                // After（正常返回）
                InvokeAfter(context);
                InvokeReturn(context);

                return new ReturnMessage(invokeResult, null, 0, callMsg.LogicalCallContext, callMsg);
            }
            catch (TargetInvocationException tex)
            {
                context.Exception = tex.InnerException ?? tex;
                InvokeAfter(context);
                InvokeException(context);
                return new ReturnMessage(context.Exception, callMsg);
            }
            catch (Exception ex)
            {
                context.Exception = ex;
                InvokeAfter(context);
                InvokeException(context);
                return new ReturnMessage(ex, callMsg);
            }
        }

        private bool ShouldIntercept(MethodInfo method)
        {
            // 检查 AOPIgnoreAttribute
            if (method.GetCustomAttribute<AOPIgnoreAttribute>() != null)
                return false;

            // 检查 AOPMethodAttribute
            var methodAttr = method.GetCustomAttribute<AOPMethodAttribute>();
            if (methodAttr != null && !methodAttr.Enable)
                return false;

            // 自定义过滤器
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
