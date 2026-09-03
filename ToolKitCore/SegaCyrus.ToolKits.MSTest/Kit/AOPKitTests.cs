using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SegaCyrus.ToolKits.Kit;
using SegaCyrus.ToolKits.Model.AOP;
using SegaCyrus.ToolKits.Model.AOP.Attributes;
using SegaCyrus.ToolKits.Package.AOPPackage.Base;
using SegaCyrus.ToolKits.Package.AOPPackage.Interface;

namespace SegaCyrus.ToolKits.MSTest.Kit
{
    /// <summary>
    /// <see cref="AOPKit"/> 单元测试
    /// </summary>
    [TestClass]
    public class AOPKitTests
    {
        #region ExecuteWithAOP

        [TestMethod]
        public void ExecuteWithAOP_Action_NormalPath_OrderCorrect()
        {
            var log = new List<string>();
            var executed = false;
            var interceptor = new AopLoggingInterceptor(log);
            AOPKit.ExecuteWithAOP(() => executed = true, interceptor);

            Assert.IsTrue(executed);
            Assert.AreEqual(3, log.Count);
            Assert.AreEqual("before:-", log[0]);
            Assert.AreEqual("after", log[1]);
            Assert.AreEqual("return", log[2]);
        }

        [TestMethod]
        public void ExecuteWithAOP_Generic_ReturnsValueAndOnReturnHasReturnValue()
        {
            var log = new List<string>();
            var result = AOPKit.ExecuteWithAOP(() => 42, new AopLoggingInterceptor(log));
            Assert.AreEqual(42, result);
            Assert.AreEqual("return", log[^1]);
        }

        [TestMethod]
        public void ExecuteWithAOP_Exception_InvokesExceptionAndAfter()
        {
            var log = new List<string>();
            Assert.ThrowsExactly<InvalidOperationException>(
                () => AOPKit.ExecuteWithAOP(() => throw new InvalidOperationException("boom"),
                    new AopLoggingInterceptor(log)));

            Assert.IsTrue(log.Contains("before:-"));
            Assert.IsTrue(log.Contains("after"));
            Assert.IsTrue(log.Contains("exception"));
            Assert.IsFalse(log.Contains("return"));
        }

        [TestMethod]
        public void ExecuteWithAOP_BeforeShortCircuit_DoesNotRunAction()
        {
            var log = new List<string>();
            var executed = false;
            AOPKit.ExecuteWithAOP(() => executed = true, new AopLoggingInterceptor(log, shortCircuit: true));
            Assert.IsFalse(executed);
            // 短路时不进入 after / return
            Assert.AreEqual(1, log.Count);
            Assert.AreEqual("before:-", log[0]);
        }

        [TestMethod]
        public void ExecuteWithAOP_Generic_ShortCircuit_ReturnsDefault()
        {
            var log = new List<string>();
            var result = AOPKit.ExecuteWithAOP<int>(() => 1, new AopLoggingInterceptor(log, shortCircuit: true));
            Assert.AreEqual(0, result);
        }

        [TestMethod]
        public void ExecuteWithAOP_MultipleInterceptors_OrderByOrder()
        {
            var log = new List<string>();
            var interceptorOrder2 = new AopLoggingInterceptor(log, order: 2);
            var interceptorOrder1 = new AopLoggingInterceptor(log, order: 1);
            AOPKit.ExecuteWithAOP(() => { }, interceptorOrder1, interceptorOrder2);

            // 优先执行 Order 小的
            Assert.AreEqual(0, log.IndexOf("before:-"));
            Assert.AreEqual(2, log.FindAll(s => s == "before:-").Count);
        }

        [TestMethod]
        public void ExecuteWithAOP_NullInterceptors_NoThrow()
        {
            AOPKit.ExecuteWithAOP(() => { }, (IAOPInterceptor[])null);
            Assert.AreEqual(7, AOPKit.ExecuteWithAOP(() => 7, (IAOPInterceptor[])null));
        }

        #endregion

        #region CreateProxy

        [TestMethod]
        public void CreateProxy_InterceptsAllMethods()
        {
            var log = new List<string>();
            var target = new AopCalc();
            var proxy = AOPKit.CreateProxy<IAopCalc>(target, new List<IAOPInterceptor>
            {
                new AopLoggingInterceptor(log)
            });

            var result = proxy.Add(2, 3);
            Assert.AreEqual(5, result);
            Assert.AreEqual(3, log.Count);
            Assert.AreEqual("before:Add", log[0]);
            Assert.AreEqual("after", log[1]);
            Assert.AreEqual("return", log[2]);

            log.Clear();
            Assert.AreEqual(1, proxy.Sub(3, 2));
            Assert.IsTrue(log.Contains("before:Sub"));
        }

        [TestMethod]
        public void CreateProxy_MethodFilter_OnlyFiltersMethodsIntercepted()
        {
            var log = new List<string>();
            var target = new AopCalc();
            var proxy = AOPKit.CreateProxy<IAopCalc>(target,
                new List<IAOPInterceptor> { new AopLoggingInterceptor(log) },
                method => method.Name == nameof(IAopCalc.Add));

            Assert.AreEqual(5, proxy.Add(2, 3));
            Assert.IsNotEmpty(log);
            Assert.IsTrue(log.Contains("before:Add"));

            log.Clear();
            Assert.AreEqual(1, proxy.Sub(3, 2));
            Assert.IsEmpty(log); // Sub 不经过拦截
        }

        [TestMethod]
        public void CreateProxy_BeforeShortCircuit_ReturnsDefault()
        {
            var target = new AopTrackingCalc();
            var proxy = AOPKit.CreateProxy<IAopCalc>(target,
                new List<IAOPInterceptor> { new ShortCircuitInterceptor(null) });

            var result = proxy.Add(1, 2);
            Assert.AreEqual(0, result); // int 默认值
            Assert.AreEqual(0, target.AddCalls); // 目标方法未执行
        }

        [TestMethod]
        public void CreateProxy_Exception_RaisesInterceptorAndPropagates()
        {
            var log = new List<string>();
            var target = new AopCalc();
            var proxy = AOPKit.CreateProxy<IAopCalc>(target,
                new List<IAOPInterceptor> { new AopLoggingInterceptor(log) });

            var ex = Assert.Throws<Exception>(() => proxy.Divide(1, 0));
            // 无论是直接异常还是包装的 TargetInvocationException，根因必须是 DivideByZero
            var root = ex is TargetInvocationException tie ? tie.InnerException : ex;
            Assert.IsInstanceOfType(root, typeof(DivideByZeroException));
            Assert.IsTrue(log.Contains("after"));
            Assert.IsTrue(log.Contains("exception"));
            Assert.IsFalse(log.Contains("return"));
        }

        [TestMethod]
        public void CreateProxy_IgnoreAndDisabledMethods_NotIntercepted()
        {
            var log = new List<string>();
            var target = new AopCalcWithAttributes();
            var proxy = AOPKit.CreateProxy<IAopCalcWithAttributes>(target,
                new List<IAOPInterceptor> { new AopLoggingInterceptor(log) });

            Assert.AreEqual(2, proxy.Add(1, 1));
            Assert.AreEqual(3, log.Count); // before + after + return

            log.Clear();
            Assert.AreEqual(1, proxy.IgnoredMethod(2, 1));
            Assert.AreEqual(6, proxy.DisabledMethod(2, 3));
            Assert.IsEmpty(log);
        }

        [TestMethod]
        public void CreateProxy_NullTargetAndInterceptorList_WorksWhenNoCall()
        {
            // 目标不可为 null；但此处验证 list 可传 null 不炸
            var target = new AopCalc();
            var proxy = AOPKit.CreateProxy<IAopCalc>(target, null, null);
            Assert.IsNotNull(proxy);
            Assert.AreEqual(5, proxy.Add(2, 3));
        }

        [TestMethod]
        public void CreateProxyByAttribute_InstantiatesInterceptorFromAttribute()
        {
            AopCountingInterceptor.Reset();
            var proxy = AOPKit.CreateProxyByAttribute<IAopMarkedService>(new AopMarkedService());
            var result = proxy.Add(10, 20);
            Assert.AreEqual(30, result);
            Assert.AreEqual(1, AopCountingInterceptor.BeforeCount);
            Assert.AreEqual(1, AopCountingInterceptor.AfterCount);
            Assert.AreEqual(1, AopCountingInterceptor.ReturnCount);
            Assert.AreEqual(0, AopCountingInterceptor.ExceptionCount);
        }

        [TestMethod]
        public void GetInterceptorsFromAttribute_ReadsAttributes()
        {
            var interceptors = AOPKit.GetInterceptorsFromAttribute<IAopMarkedService>();
            Assert.IsNotNull(interceptors);
            Assert.AreEqual(1, interceptors.Count);
            Assert.IsInstanceOfType(interceptors[0], typeof(AopCountingInterceptor));
        }

        #endregion

        #region 参数 / 选项

        [TestMethod]
        public void CreateProxy_InterfaceTypeNotInterface_Throws()
        {
            Assert.ThrowsExactly<ArgumentException>(
                () => AOPKit.CreateProxy<AopCalc>(new AopCalc(), new List<IAOPInterceptor>()));
        }

        [TestMethod]
        public void CreateProxyByAttribute_NoAttribute_PassesThrough()
        {
            // IAopCalc 没有 [AOP] 特性 -> 无拦截器，直接透传
            var proxy = AOPKit.CreateProxyByAttribute<IAopCalc>(new AopCalc());
            Assert.IsNotNull(proxy);
            Assert.AreEqual(5, proxy.Add(2, 3));
        }

        [TestMethod]
        public void AOPProxyOptions_DefaultsAreUsable()
        {
            var options = new AOPProxyOptions();
            Assert.IsNull(options.Target);
            Assert.IsNull(options.InterfaceType);
            Assert.IsNull(options.MethodFilter);
            Assert.IsNotNull(options.Interceptors);
            Assert.IsEmpty(options.Interceptors);
        }

        [TestMethod]
        public void AOPAttribute_WithNonInterceptorType_Throws()
        {
            Assert.ThrowsExactly<ArgumentException>(() => new AOPAttribute(typeof(string)));
        }

        [TestMethod]
        public void AOPInterceptorBase_DefaultsWork()
        {
            var baseInterceptor = new DummyInterceptor();
            Assert.AreEqual(0, baseInterceptor.Order);
            var context = new AOPInterceptContext();
            Assert.IsTrue(baseInterceptor.OnBefore(context));
            baseInterceptor.OnAfter(context);
            baseInterceptor.OnReturn(context);
            baseInterceptor.OnException(context);
        }

        #endregion
    }

    /// <summary>
    /// 短路拦截器
    /// </summary>
    public sealed class ShortCircuitInterceptor : IAOPInterceptor
    {
        private readonly Action _action;
        public ShortCircuitInterceptor(Action action) => _action = action;
        public int Order => 0;
        public bool OnBefore(AOPInterceptContext context)
        {
            _action?.Invoke();
            return false;
        }
        public void OnAfter(AOPInterceptContext context) { }
        public void OnReturn(AOPInterceptContext context) { }
        public void OnException(AOPInterceptContext context) { }
    }

    /// <summary>仅测试默认行为</summary>
    public sealed class DummyInterceptor : AOPInterceptorBase
    {
    }
}
