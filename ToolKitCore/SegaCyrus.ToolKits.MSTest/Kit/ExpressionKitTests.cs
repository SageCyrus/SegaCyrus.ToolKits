using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SegaCyrus.ToolKits.Kit;
using SegaCyrus.ToolKits.Model.Expression;
using TestModels = SegaCyrus.ToolKits.MSTest.Kit.TestModels;

namespace SegaCyrus.ToolKits.MSTest.Kit
{
    /// <summary>
    /// <see cref="ExpressionKit"/> 单元测试
    /// </summary>
    [TestClass]
    public class ExpressionKitTests
    {
        #region 字段访问器

        [TestMethod]
        public void CreateFieldExpressWithoutCache_SetGet_RoundTrip()
        {
            var model = ExpressionKit.CreateFieldExpressWithoutCache<TestModels.ReflectSample, int>(
                typeof(TestModels.ReflectSample), nameof(TestModels.ReflectSample.InstanceField));

            Assert.IsNotNull(model);
            Assert.AreEqual(nameof(TestModels.ReflectSample.InstanceField), model.FieldName);
            Assert.IsNotNull(model.Set);
            Assert.IsNotNull(model.Get);

            var sample = new TestModels.ReflectSample();
            model.Set(sample, 321);
            Assert.AreEqual(321, model.Get(sample));
            Assert.AreEqual(321, sample.InstanceField);
        }

        [TestMethod]
        public void CreateFieldExpressWithoutCache_PrivateField_Works()
        {
            var model = ExpressionKit.CreateFieldExpressWithoutCache<TestModels.ReflectSample, string>(
                typeof(TestModels.ReflectSample), "_secret");
            var sample = new TestModels.ReflectSample();
            model.Set(sample, "updated-secret");
            Assert.AreEqual("updated-secret", model.Get(sample));
        }

        [TestMethod]
        public void CreateFieldExpressWithoutCache_MissingField_Throws()
        {
            var ex = Assert.ThrowsExactly<ArgumentNullException>(
                () => ExpressionKit.CreateFieldExpressWithoutCache<TestModels.ReflectSample, int>(
                    typeof(TestModels.ReflectSample), "NoSuchField"));
            StringAssert.Contains(ex.Message, "不存在字段");
        }

        [TestMethod]
        public void CreateFieldExpressWithoutCache_NullType_Throws()
        {
            Assert.ThrowsExactly<ArgumentNullException>(
                () => ExpressionKit.CreateFieldExpressWithoutCache<TestModels.ReflectSample, int>((Type)null, "x"));
        }

        [TestMethod]
        public void CreateFieldExpress_InstanceOverload_CachedWorks()
        {
            var model = ExpressionKit.CreateFieldExpress<TestModels.ReflectSample, int>(new TestModels.ReflectSample(),
                nameof(TestModels.ReflectSample.InstanceField));
            var model2 = ExpressionKit.CreateFieldExpress<TestModels.ReflectSample, int>(new TestModels.ReflectSample(),
                nameof(TestModels.ReflectSample.InstanceField));
            Assert.AreSame(model, model2);

            var sample = new TestModels.ReflectSample();
            model.Set(sample, 555);
            Assert.AreEqual(555, model.Get(sample));
        }

        #endregion

        #region 属性访问器

        [TestMethod]
        public void CreatePropertyExpressWithoutCache_SetGet_RoundTrip()
        {
            var model = ExpressionKit.CreatePropertyExpressWithoutCache<TestModels.ReflectSample, int>(
                typeof(TestModels.ReflectSample), nameof(TestModels.ReflectSample.Number));

            Assert.IsNotNull(model);
            Assert.AreEqual(nameof(TestModels.ReflectSample.Number), model.FieldName);

            var sample = new TestModels.ReflectSample();
            model.Set(sample, 66);
            Assert.AreEqual(66, model.Get(sample));
            Assert.AreEqual(66, sample.Number);
        }

        [TestMethod]
        public void CreatePropertyExpressWithoutCache_StringProperty_Works()
        {
            var model = ExpressionKit.CreatePropertyExpressWithoutCache<TestModels.ReflectSample, string>(
                typeof(TestModels.ReflectSample), nameof(TestModels.ReflectSample.Name));
            var sample = new TestModels.ReflectSample();
            model.Set(sample, "some-name");
            Assert.AreEqual("some-name", model.Get(sample));
            Assert.AreEqual("some-name", sample.Name);
        }

        [TestMethod]
        public void CreatePropertyExpress_TypeOverload_Cached()
        {
            var model = ExpressionKit.CreatePropertyExpress<TestModels.ReflectSample, string>(
                typeof(TestModels.ReflectSample), nameof(TestModels.ReflectSample.Name));
            var model2 = ExpressionKit.CreatePropertyExpress<TestModels.ReflectSample, string>(
                typeof(TestModels.ReflectSample), nameof(TestModels.ReflectSample.Name));
            Assert.AreSame(model, model2);
        }

        [TestMethod]
        public void CreatePropertyExpress_MissingProperty_Throws()
        {
            var ex = Assert.ThrowsExactly<ArgumentNullException>(
                () => ExpressionKit.CreatePropertyExpressWithoutCache<TestModels.ReflectSample, int>(
                    typeof(TestModels.ReflectSample), "NoSuchProperty"));
            StringAssert.Contains(ex.Message, "不存在属性");
        }

        #endregion

        #region 模型一致性

        [TestMethod]
        public void ExpressionGetSetModel_IsPublicWithReadonlyMembers()
        {
            var t = typeof(ExpressionGetSetModel<TestModels.ReflectSample, int>);
            Assert.IsTrue(t.IsPublic);
            Assert.IsNotNull(t.GetField("FieldName"));
            Assert.IsNotNull(t.GetField("Set"));
            Assert.IsNotNull(t.GetField("Get"));
        }

        #endregion
    }
}
