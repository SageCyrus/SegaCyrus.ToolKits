using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SegaCyrus.ToolKits.Kit;

namespace SegaCyrus.ToolKits.MSTest.Kit
{
    /// <summary>
    /// <see cref="AssertKit"/> 单元测试
    /// </summary>
    [TestClass]
    public class AssertKitTests
    {
        #region AssertTrue

        [TestMethod]
        public void AssertTrue_WithTrue_DoesNotThrow()
        {
            AssertKit.AssertTrue(true);
        }

        [TestMethod]
        public void AssertTrue_WithFalse_ThrowsArgumentException()
        {
            var ex = Assert.ThrowsExactly<ArgumentException>(() => AssertKit.AssertTrue(false));
            StringAssert.Contains(ex.Message, "必须为True");
        }

        [TestMethod]
        public void AssertTrue_WithFalseAndCustomMsg_UsesCustomMsg()
        {
            var ex = Assert.ThrowsExactly<ArgumentException>(() => AssertKit.AssertTrue(false, nameof(AssertTrue_WithFalseAndCustomMsg_UsesCustomMsg), "自定义错误"));
            Assert.AreEqual("自定义错误", ex.Message);
        }

        #endregion

        #region AssertFalse

        [TestMethod]
        public void AssertFalse_WithFalse_ReturnsFalse()
        {
            Assert.IsFalse(AssertKit.AssertFalse(false));
        }

        [TestMethod]
        public void AssertFalse_WithTrue_ThrowsArgumentException()
        {
            var ex = Assert.ThrowsExactly<ArgumentException>(() => AssertKit.AssertFalse(true));
            StringAssert.Contains(ex.Message, "必须为False");
        }

        #endregion

        #region AssertPositive

        [TestMethod]
        public void AssertPositive_WithPositive_ReturnsData()
        {
            Assert.AreEqual(5, AssertKit.AssertPositive(5));
        }

        [TestMethod]
        public void AssertPositive_WithZero_Throws()
        {
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => AssertKit.AssertPositive(0));
        }

        [TestMethod]
        public void AssertPositive_WithNegative_Throws()
        {
            var ex = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => AssertKit.AssertPositive(-1, nameof(AssertPositive_WithNegative_Throws)));
            StringAssert.Contains(ex.Message, "-1");
        }

        #endregion

        #region AssertGreater

        [TestMethod]
        public void AssertGreater_DataGreater_ReturnsData()
        {
            Assert.AreEqual(10, AssertKit.AssertGreater(10, 5));
        }

        [TestMethod]
        public void AssertGreater_DataEqual_Throws()
        {
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => AssertKit.AssertGreater(5, 5));
        }

        [TestMethod]
        public void AssertGreater_DataLesser_Throws()
        {
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => AssertKit.AssertGreater(3, 5));
        }

        #endregion

        #region AssertLesser

        [TestMethod]
        public void AssertLesser_DataLesser_ReturnsData()
        {
            Assert.AreEqual(3, AssertKit.AssertLesser(3, 5));
        }

        [TestMethod]
        public void AssertLesser_DataEqual_Throws()
        {
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => AssertKit.AssertLesser(5, 5));
        }

        [TestMethod]
        public void AssertLesser_DataGreater_Throws()
        {
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => AssertKit.AssertLesser(6, 5));
        }

        #endregion

        #region AssertEQGreater

        [TestMethod]
        public void AssertEQGreater_DataGreater_ReturnsData()
        {
            Assert.AreEqual(10, AssertKit.AssertEQGreater(10, 5));
        }

        [TestMethod]
        public void AssertEQGreater_DataEqual_ReturnsData()
        {
            Assert.AreEqual(5, AssertKit.AssertEQGreater(5, 5));
        }

        [TestMethod]
        public void AssertEQGreater_DataLesser_Throws()
        {
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => AssertKit.AssertEQGreater(4, 5));
        }

        #endregion

        #region AssertEQLesser

        [TestMethod]
        public void AssertEQLesser_DataLesser_ReturnsData()
        {
            Assert.AreEqual(4, AssertKit.AssertEQLesser(4, 5));
        }

        [TestMethod]
        public void AssertEQLesser_DataEqual_ReturnsData()
        {
            Assert.AreEqual(5, AssertKit.AssertEQLesser(5, 5));
        }

        [TestMethod]
        public void AssertEQLesser_DataGreater_Throws()
        {
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => AssertKit.AssertEQLesser(6, 5));
        }

        #endregion

        #region AssertRange

        [TestMethod]
        public void AssertRange_Inside_ReturnsData()
        {
            Assert.AreEqual(5, AssertKit.AssertRange(5, 1, 10));
        }

        [TestMethod]
        public void AssertRange_OnBoundary_ReturnsData()
        {
            Assert.AreEqual(1, AssertKit.AssertRange(1, 1, 10));
            Assert.AreEqual(10, AssertKit.AssertRange(10, 1, 10));
        }

        [TestMethod]
        public void AssertRange_BelowMin_Throws()
        {
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => AssertKit.AssertRange(0, 1, 10));
        }

        [TestMethod]
        public void AssertRange_AboveMax_Throws()
        {
            var ex = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => AssertKit.AssertRange(11, 1, 10));
            StringAssert.Contains(ex.Message, "不满足约束大于等于1或小于等于10");
        }

        [TestMethod]
        public void AssertRange_DateTimeRange()
        {
            var min = new DateTime(2020, 1, 1);
            var max = new DateTime(2020, 12, 31);
            var mid = new DateTime(2020, 6, 1);
            Assert.AreEqual(mid, AssertKit.AssertRange(mid, min, max));
        }

        #endregion

        #region AssertNotNull

        [TestMethod]
        public void AssertNotNull_NonNull_ReturnsData()
        {
            var obj = new object();
            Assert.AreSame(obj, AssertKit.AssertNotNull(obj));
            Assert.AreEqual("hi", AssertKit.AssertNotNull("hi"));
        }

        [TestMethod]
        public void AssertNotNull_Null_Throws()
        {
            Assert.ThrowsExactly<ArgumentNullException>(() => AssertKit.AssertNotNull<object>(null));
        }

        [TestMethod]
        public void AssertNotNull_NullString_Throws()
        {
            Assert.ThrowsExactly<ArgumentNullException>(() => AssertKit.AssertNotNull<string>(null, "myParam"));
        }

        #endregion

        #region AssertNotEmpty (集合)

        [TestMethod]
        public void AssertNotEmpty_Collection_WithElements_ReturnsData()
        {
            var list = new List<int> { 1, 2, 3 };
            Assert.AreSame(list, AssertKit.AssertNotEmpty(list));
        }

        [TestMethod]
        public void AssertNotEmpty_EmptyCollection_Throws()
        {
            Assert.ThrowsExactly<ArgumentException>(() => AssertKit.AssertNotEmpty(new int[0]));
        }

        [TestMethod]
        public void AssertNotEmpty_NullCollection_Throws()
        {
            Assert.ThrowsExactly<ArgumentException>(() => AssertKit.AssertNotEmpty((IEnumerable<int>)null));
        }

        #endregion

        #region AssertNotEmpty (字符串)

        [TestMethod]
        public void AssertNotEmpty_String_WithValue_ReturnsData()
        {
            Assert.AreEqual("value", AssertKit.AssertNotEmpty("value"));
        }

        [TestMethod]
        public void AssertNotEmpty_EmptyString_Throws()
        {
            Assert.ThrowsExactly<ArgumentException>(() => AssertKit.AssertNotEmpty(string.Empty));
        }

        [TestMethod]
        public void AssertNotEmpty_NullString_Throws()
        {
            var ex = Assert.ThrowsExactly<ArgumentException>(() => AssertKit.AssertNotEmpty((string)null));
            StringAssert.Contains(ex.Message, "为空");
        }

        [TestMethod]
        public void AssertNotEmpty_WhitespaceString_IsNotEmpty()
        {
            // 仅空白不算空
            Assert.AreEqual("   ", AssertKit.AssertNotEmpty("   "));
        }

        #endregion
    }
}
