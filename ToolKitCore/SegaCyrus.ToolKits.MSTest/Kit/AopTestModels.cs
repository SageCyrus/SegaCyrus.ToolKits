using System.Collections.Generic;
using SegaCyrus.ToolKits.Model.AOP;
using SegaCyrus.ToolKits.Model.AOP.Attributes;
using SegaCyrus.ToolKits.Package.AOPPackage.Base;

namespace SegaCyrus.ToolKits.MSTest.Kit
{
    /// <summary>
    /// AOP 测试目标接口
    /// </summary>
    public interface IAopCalc
    {
        int Add(int a, int b);
        int Sub(int a, int b);
        int Divide(int a, int b);
    }

    /// <summary>带特性与忽略/禁用方法的目标接口</summary>
    public interface IAopCalcWithAttributes
    {
        int Add(int a, int b);

        [AOPIgnore]
        int IgnoredMethod(int a, int b);

        [AOPMethod(Enable = false)]
        int DisabledMethod(int a, int b);
    }

    /// <summary>标记 AOPAttribute 的接口（用于 CreateProxyByAttribute）</summary>
    [AOP(typeof(AopCountingInterceptor))]
    public interface IAopMarkedService
    {
        int Add(int a, int b);
    }

    public class AopCalc : IAopCalc
    {
        public int Add(int a, int b) => a + b;
        public int Sub(int a, int b) => a - b;
        public int Divide(int a, int b) => a / b;
    }

    /// <summary>记录调用次数的目标类</summary>
    public class AopTrackingCalc : IAopCalc
    {
        public int AddCalls { get; private set; }
        public int Add(int a, int b)
        {
            AddCalls++;
            return a + b;
        }
        public int Sub(int a, int b) => a - b;
        public int Divide(int a, int b) => a / b;
    }

    public class AopCalcWithAttributes : IAopCalcWithAttributes
    {
        public int Add(int a, int b) => a + b;
        public int IgnoredMethod(int a, int b) => a - b;
        public int DisabledMethod(int a, int b) => a * b;
    }

    public class AopMarkedService : IAopMarkedService
    {
        public int Add(int a, int b) => a + b;
    }

    /// <summary>基础日志拦截器：记录各阶段调用，可配置是否短路</summary>
    public class AopLoggingInterceptor : AOPInterceptorBase
    {
        private readonly List<string> _log;
        private readonly bool _shortCircuit;
        public override int Order { get; }
        public int BeforeCount { get; private set; }

        public AopLoggingInterceptor(List<string> log, int order = 0, bool shortCircuit = false)
        {
            _log = log;
            _shortCircuit = shortCircuit;
            Order = order;
        }

        public override bool OnBefore(AOPInterceptContext context)
        {
            BeforeCount++;
            lock (_log) _log.Add($"before:{context.Method?.Name ?? "-"}");
            return !_shortCircuit;
        }

        public override void OnAfter(AOPInterceptContext context)
        {
            lock (_log) _log.Add("after");
        }

        public override void OnReturn(AOPInterceptContext context)
        {
            lock (_log) _log.Add("return");
        }

        public override void OnException(AOPInterceptContext context)
        {
            lock (_log) _log.Add("exception");
        }
    }

    /// <summary>带默认构造、基于静态计数器的拦截器（Attribute 创建需要默认构造）</summary>
    public class AopCountingInterceptor : AOPInterceptorBase
    {
        public static int BeforeCount;
        public static int AfterCount;
        public static int ReturnCount;
        public static int ExceptionCount;

        public static void Reset()
        {
            BeforeCount = 0;
            AfterCount = 0;
            ReturnCount = 0;
            ExceptionCount = 0;
        }

        public override bool OnBefore(AOPInterceptContext context)
        {
            BeforeCount++;
            return true;
        }

        public override void OnAfter(AOPInterceptContext context) => AfterCount++;
        public override void OnReturn(AOPInterceptContext context) => ReturnCount++;
        public override void OnException(AOPInterceptContext context) => ExceptionCount++;
    }
}
