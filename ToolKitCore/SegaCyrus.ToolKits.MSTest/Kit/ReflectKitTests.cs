using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SegaCyrus.ToolKits.Kit;
using SegaCyrus.ToolKits.Model.Common;
using SegaCyrus.ToolKits.Model.ReflectInvoke;
using TestModels = SegaCyrus.ToolKits.MSTest.Kit.TestModels;

namespace SegaCyrus.ToolKits.MSTest.Kit
{
    /// <summary>
    /// <see cref="ReflectKit"/> 单元测试
    /// </summary>
    [TestClass]
    public class ReflectKitTests
    {
        #region GetTypeByFullName

        [TestMethod]
        public void GetTypeByFullName_WithFullName_ReturnsType()
        {
            // 用主库中的类型（GetTypeByFullName 仅能扫描到当前可加载的程序集）
            var type = ReflectKit.GetTypeByFullName(typeof(CodeKit).FullName);
            Assert.IsNotNull(type);
            Assert.AreEqual(typeof(CodeKit), type);
        }

        [TestMethod]
        public void GetTypeByFullName_IgnoreCaseWorks()
        {
            var fullName = typeof(CodeKit).FullName.ToLowerInvariant();
            var type = ReflectKit.GetTypeByFullName(fullName, true);
            Assert.IsNotNull(type);
            Assert.AreEqual(typeof(CodeKit), type);
        }

        [TestMethod]
        public void GetTypeByFullName_NotExist_ReturnsNull()
        {
            Assert.IsNull(ReflectKit.GetTypeByFullName("System.NotExist.TypeXyz"));
        }

        #endregion

        #region GetDefaultValue

        [TestMethod]
        public void GetDefaultValue_ValueType_ReturnsDefault()
        {
            Assert.AreEqual(0, ReflectKit.GetDefaultValue(typeof(int)));
            Assert.AreEqual(false, ReflectKit.GetDefaultValue(typeof(bool)));
            Assert.AreEqual(default(DateTime), ReflectKit.GetDefaultValue(typeof(DateTime)));
        }

        [TestMethod]
        public void GetDefaultValue_ReferenceType_ReturnsNull()
        {
            Assert.IsNull(ReflectKit.GetDefaultValue(typeof(string)));
        }

        #endregion

        #region 实例创建

        [TestMethod]
        public void CreateInstance_Generic_ReturnsInstance()
        {
            var obj = ReflectKit.CreateInstance<TestModels.ReflectSample>();
            Assert.IsNotNull(obj);
            Assert.IsInstanceOfType(obj, typeof(TestModels.ReflectSample));
        }

        [TestMethod]
        public void CreateInstance_WithArgs_ReturnsInstance()
        {
            var obj = ReflectKit.CreateInstance(typeof(TestModels.CtorSample), new object[] { 5 });
            Assert.IsNotNull(obj);
            Assert.AreEqual(5, ((TestModels.CtorSample)obj).X);
        }

        [TestMethod]
        public void CreateInstanceWithDefaultValue_NoParameterlessCtor_UsesDefaultArgs()
        {
            var obj = ReflectKit.CreateInstanceWithDefaultValue<TestModels.CtorSample>();
            Assert.IsNotNull(obj);
            Assert.AreEqual(0, obj.X);
        }

        #endregion

        #region IsInhert

        [TestMethod]
        public void IsInhert_WithGenerics_ChecksDerived()
        {
            Assert.IsTrue(ReflectKit.IsInhert<TestModels.MarkedBase>(typeof(TestModels.MarkedDerived)));
            Assert.IsFalse(ReflectKit.IsInhert<TestModels.MarkedDerived>(typeof(TestModels.MarkedBase)));
        }

        [TestMethod]
        public void IsInhert_WithTypeAndParentType()
        {
            Assert.IsTrue(ReflectKit.IsInhert(typeof(TestModels.MarkedDerived), typeof(TestModels.MarkedBase)));
            Assert.IsTrue(ReflectKit.IsInhert(typeof(TestModels.MarkedBase), typeof(TestModels.MarkedBase)));
            Assert.IsFalse(ReflectKit.IsInhert(typeof(string), typeof(TestModels.MarkedBase)));
        }

        #endregion

        #region 集合类型判断

        [TestMethod]
        public void IsList_VariousTypes()
        {
            Assert.IsTrue(ReflectKit.IsList(typeof(List<int>)));
            // 数组实现了非泛型 IList
            Assert.IsTrue(ReflectKit.IsList(typeof(int[])));
            Assert.IsFalse(ReflectKit.IsList(typeof(string)));
        }

        [TestMethod]
        public void IsEnumerable_VariousTypes()
        {
            Assert.IsTrue(ReflectKit.IsEnumerable(typeof(List<int>)));
            Assert.IsTrue(ReflectKit.IsEnumerable(typeof(int[])));
            Assert.IsTrue(ReflectKit.IsEnumerable(typeof(string)));
            Assert.IsTrue(ReflectKit.IsEnumerable(typeof(Dictionary<string, int>)));
            Assert.IsFalse(ReflectKit.IsEnumerable(typeof(int)));
        }

        #endregion

        #region 程序集

        [TestMethod]
        public void GetAssemblies_ReturnsResult()
        {
            var result = ReflectKit.GetAssemblies();
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.ExistAssemblies);
            Assert.IsNotNull(result.NotExistAssemblies);
            // 至少包含测试程序集引用或可加载程序集
            Assert.IsNotEmpty(result.ExistAssemblies);
        }

        [TestMethod]
        public void GetTypesByAssemble_GivenAssembly_ReturnsTypes()
        {
            var asm = Assembly.GetExecutingAssembly();
            var types = ReflectKit.GetTypesByAssemble(asm);
            Assert.IsNotNull(types);
            CollectionAssert.Contains(types, typeof(TestModels.ReflectSample));
        }

        #endregion

        #region 特性+继承查询

        [TestMethod]
        public void GetMarkAttrAndInhertClassTypes_FindsMarkedTypes()
        {
            var result = ReflectKit.GetMarkAttrAndInhertClassTypes<TestModels.MarkedBase, TestModels.MarkAttr>(
                true, Assembly.GetExecutingAssembly());

            Assert.IsNotNull(result);
            Assert.IsNotEmpty(result);
            var types = result.Select(c => c.Key).ToList();
            CollectionAssert.Contains(types, typeof(TestModels.MarkedBase));
            CollectionAssert.Contains(types, typeof(TestModels.MarkedDerived));
            CollectionAssert.DoesNotContain(types, typeof(TestModels.PlainDerived));
        }

        [TestMethod]
        public void GetMarkAttrAndInhertClassTypes_ReturnsKVModelPairs()
        {
            var result = ReflectKit.GetMarkAttrAndInhertClassTypes<TestModels.MarkedBase, TestModels.MarkAttr>(
                true, Assembly.GetExecutingAssembly());
            var pair = result.FirstOrDefault(c => c.Key == typeof(TestModels.MarkedBase));
            Assert.IsNotNull(pair);
            Assert.IsInstanceOfType(pair, typeof(KVModel<Type, TestModels.MarkAttr>));
            Assert.IsNotNull(pair.Value);
        }

        #endregion
    }
}
