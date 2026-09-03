using System;
using System.Threading;

namespace SegaCyrus.ToolKits.MSTest.Kit
{
    /// <summary>
    /// 仅用于单元测试的公共示例类型
    /// </summary>
    public static class TestModels
    {
        /// <summary>测试特性</summary>
        [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
        public sealed class MarkAttr : Attribute
        {
        }

        [MarkAttr]
        public class MarkedBase
        {
        }

        [MarkAttr]
        public class MarkedDerived : MarkedBase
        {
        }

        public class PlainDerived : MarkedBase
        {
        }

        /// <summary>测试继承的类</summary>
        public class ReflectSample
        {
            public static int StaticField;
            public static string StaticAutoProperty { get; set; }
            public static string StaticTextProperty { get; set; } = "static-default";

            public int InstanceField;
#pragma warning disable CS0414 // 通过反射访问的字段
            private string _secret = "initial-secret";
#pragma warning restore CS0414

            public int Number { get; set; }
            public string Name { get; set; }
            public string ReadOnlyName => "fixed";

            public static int StaticAdd(int a, int b) => a + b;
            public int Add(int a, int b) => a + b;
            public string Merge(string a, string b) => a + b;
            public int Sum(int a, int b = 10, int c = 5) => a + b + c;
            public T Echo<T>(T input) => input;
            public static T StaticEcho<T>(T input) => input;
        }

        /// <summary>带参构造类型</summary>
        public class CtorSample
        {
            public int X { get; }
            public CtorSample(int x) => X = x;
        }
    }
}
