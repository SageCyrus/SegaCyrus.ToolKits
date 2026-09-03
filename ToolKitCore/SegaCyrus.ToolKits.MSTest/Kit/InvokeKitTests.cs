using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SegaCyrus.ToolKits.Kit;
using TestModels = SegaCyrus.ToolKits.MSTest.Kit.TestModels;

namespace SegaCyrus.ToolKits.MSTest.Kit
{
    /// <summary>
    /// <see cref="InvokeKit"/> 单元测试
    /// </summary>
    [TestClass]
    public class InvokeKitTests
    {
        private static string FullNameOfSample => typeof(TestModels.ReflectSample).FullName;

        #region 静态属性 / 字段

        [TestMethod]
        public void SetStaticProperty_GetStaticProperty_RoundTrip()
        {
            var type = typeof(TestModels.ReflectSample);
            InvokeKit.SetStaticProperty(type, nameof(TestModels.ReflectSample.StaticAutoProperty), "abc");
            Assert.AreEqual("abc", InvokeKit.GetStaticProperty(type, nameof(TestModels.ReflectSample.StaticAutoProperty)));
            // 还原
            TestModels.ReflectSample.StaticAutoProperty = null;
        }

        [TestMethod]
        public void GetStaticProperty_MissingProperty_Throws()
        {
            Assert.ThrowsExactly<ArgumentNullException>(
                () => InvokeKit.GetStaticProperty(typeof(TestModels.ReflectSample), "NotExistProperty"));
        }

        [TestMethod]
        public void SetStaticField_GetStaticField_RoundTrip()
        {
            var type = typeof(TestModels.ReflectSample);
            InvokeKit.SetStaticField(type, nameof(TestModels.ReflectSample.StaticField), 42);
            Assert.AreEqual(42, InvokeKit.GetStaticField(type, nameof(TestModels.ReflectSample.StaticField)));
        }

        #endregion

        #region 实例属性 / 字段

        [TestMethod]
        public void SetProperty_GetProperty_RoundTrip()
        {
            var sample = new TestModels.ReflectSample();
            InvokeKit.SetProperty(sample, nameof(TestModels.ReflectSample.Number), 7);
            Assert.AreEqual(7, InvokeKit.GetProperty(sample, nameof(TestModels.ReflectSample.Number)));
        }

        [TestMethod]
        public void SetField_GetField_RoundTrip()
        {
            var sample = new TestModels.ReflectSample();
            InvokeKit.SetField(sample, nameof(TestModels.ReflectSample.InstanceField), 100);
            Assert.AreEqual(100, InvokeKit.GetField(sample, nameof(TestModels.ReflectSample.InstanceField)));
        }

        [TestMethod]
        public void GetProperty_Missing_Throws()
        {
            Assert.ThrowsExactly<ArgumentNullException>(
                () => InvokeKit.GetProperty(new TestModels.ReflectSample(), "NotExist"));
        }

        #endregion

        #region 实例方法调用

        [TestMethod]
        public void CallMethod_WithParams_ReturnsResult()
        {
            var sample = new TestModels.ReflectSample();
            Assert.AreEqual(5, InvokeKit.CallMethod(sample, nameof(TestModels.ReflectSample.Add), 2, 3));
            Assert.AreEqual("ab", InvokeKit.CallMethod(sample, nameof(TestModels.ReflectSample.Merge), "a", "b"));
        }

        [TestMethod]
        public void CallMethod_WithDefaultParams_FillsDefaults()
        {
            var sample = new TestModels.ReflectSample();
            Assert.AreEqual(16, InvokeKit.CallMethod(sample, nameof(TestModels.ReflectSample.Sum), 1));
        }

        [TestMethod]
        public void CallGenericMethod_Works()
        {
            var sample = new TestModels.ReflectSample();
            var result = InvokeKit.CallGenericMethod(sample, nameof(TestModels.ReflectSample.Echo),
                new List<Type> { typeof(string) }, "hello");
            Assert.AreEqual("hello", result);
        }

        [TestMethod]
        public void CallMethod_MissingMethod_Throws()
        {
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(
                () => InvokeKit.CallMethod(new TestModels.ReflectSample(), "NotExistMethod"));
        }

        #endregion

        #region 静态方法调用

        [TestMethod]
        public void CallStaticMethod_WithFullName_Works()
        {
            Assert.AreEqual(3, InvokeKit.CallStaticMethod(FullNameOfSample, nameof(TestModels.ReflectSample.StaticAdd), 1, 2));
        }

        [TestMethod]
        public void CallStaticMethod_WithType_Works()
        {
            Assert.AreEqual(9, InvokeKit.CallStaticMethod(typeof(TestModels.ReflectSample), nameof(TestModels.ReflectSample.StaticAdd), 4, 5));
        }

        [TestMethod]
        public void CallGenericStaticMethod_WithType_Works()
        {
            var result = InvokeKit.CallGenericStaticMethod(typeof(TestModels.ReflectSample),
                nameof(TestModels.ReflectSample.StaticEcho), new List<Type> { typeof(int) }, 77);
            Assert.AreEqual(77, result);
        }

        [TestMethod]
        public void CallGenericStaticMethod_WithFullName_Works()
        {
            var result = InvokeKit.CallGenericStaticMethod(FullNameOfSample,
                nameof(TestModels.ReflectSample.StaticEcho), new List<Type> { typeof(double) }, 3.5);
            Assert.AreEqual(3.5, result);
        }

        #endregion
    }
}
