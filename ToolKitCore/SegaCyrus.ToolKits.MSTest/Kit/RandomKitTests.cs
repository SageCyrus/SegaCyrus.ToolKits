using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SegaCyrus.ToolKits.Kit;

namespace SegaCyrus.ToolKits.MSTest.Kit
{
    /// <summary>
    /// <see cref="RandomKit"/> 单元测试（代表性确定性断言）
    /// </summary>
    [TestClass]
    public class RandomKitTests
    {
        [TestMethod]
        public void NextInt_WithinRange()
        {
            for (var i = 0; i < 200; i++)
            {
                var value = RandomKit.NextInt(3, 8);
                Assert.IsTrue(value >= 3 && value < 8, $"值 {value} 超出 [3,8)");
            }
        }

        [TestMethod]
        public void NextInt_InvalidRange_Throws()
        {
            Assert.ThrowsExactly<ArgumentException>(() => RandomKit.NextInt(5, 5));
            Assert.ThrowsExactly<ArgumentException>(() => RandomKit.NextInt(9, 5));
        }

        [TestMethod]
        public void NextLong_WithinRange()
        {
            for (var i = 0; i < 200; i++)
            {
                var value = RandomKit.NextLong(100, 200);
                Assert.IsTrue(value >= 100 && value < 200, $"值 {value} 超出 [100,200)");
            }
        }

        [TestMethod]
        public void NextDouble_WithinRange_AndRounded()
        {
            for (var i = 0; i < 100; i++)
            {
                var value = RandomKit.NextDouble(0.5, 1.5);
                Assert.IsTrue(value >= 0.5 && value < 1.5);
            }
            var rounded = RandomKit.NextDouble(0, 1, 2);
            Assert.AreEqual(rounded, Math.Round(rounded, 2));
        }

        [TestMethod]
        public void NextFloat_WithinRange()
        {
            var value = RandomKit.NextFloat(-1, 1);
            Assert.IsTrue(value >= -1 && value < 1);
        }

        [TestMethod]
        public void NextDecimal_WithinRange()
        {
            for (var i = 0; i < 100; i++)
            {
                var value = RandomKit.NextDecimal(10m, 20m);
                Assert.IsTrue(value >= 10m && value < 20m, $"值 {value} 超出范围");
            }
        }

        [TestMethod]
        public void NextBool_ExtremeProbabilities()
        {
            for (var i = 0; i < 50; i++) Assert.IsFalse(RandomKit.NextBool(0));
            for (var i = 0; i < 50; i++) Assert.IsTrue(RandomKit.NextBool(1));
        }

        [TestMethod]
        public void NextBool_InvalidProbability_Throws()
        {
            Assert.ThrowsExactly<ArgumentException>(() => RandomKit.NextBool(-0.1));
            Assert.ThrowsExactly<ArgumentException>(() => RandomKit.NextBool(1.1));
        }

        [TestMethod]
        public void NextChar_FromPool()
        {
            const string pool = "abc";
            for (var i = 0; i < 200; i++)
            {
                var c = RandomKit.NextChar(pool);
                Assert.IsTrue(pool.Contains(c));
            }
        }

        [TestMethod]
        public void NextString_WithLength_UsesPool()
        {
            const string pool = "XYZ012";
            for (var i = 0; i < 20; i++)
            {
                var s = RandomKit.NextString(10, pool);
                Assert.AreEqual(10, s.Length);
                Assert.IsTrue(s.All(c => pool.Contains(c)));
            }
        }

        [TestMethod]
        public void NextString_RangeLength()
        {
            for (var i = 0; i < 50; i++)
            {
                var s = RandomKit.NextString(3, 6);
                Assert.IsTrue(s.Length >= 3 && s.Length <= 6, $"长度 {s.Length} 超出 [3,6]");
            }
        }

        [TestMethod]
        public void NextGuid_ReturnsValidGuid()
        {
            var guid = RandomKit.NextGuid();
            Assert.AreNotEqual(Guid.Empty, guid);
        }

        [TestMethod]
        public void NextGuidString_FormatN_Returns32()
        {
            var s = RandomKit.NextGuidString("N");
            Assert.AreEqual(32, s.Length);
            Assert.IsTrue(Guid.TryParseExact(s, "N", out _));
        }

        [TestMethod]
        public void NextItem_Array_ReturnsElementFromArray()
        {
            var arr = new[] { 10, 20, 30, 40 };
            for (var i = 0; i < 100; i++)
            {
                var v = RandomKit.NextItem(arr);
                Assert.IsTrue(arr.Contains(v));
            }
        }

        [TestMethod]
        public void NextItem_IList_ReturnsElement()
        {
            IList<string> list = new List<string> { "a", "b", "c" };
            for (var i = 0; i < 100; i++)
            {
                Assert.IsTrue(list.Contains(RandomKit.NextItem(list)));
            }
        }

        [TestMethod]
        public void NextSample_ReturnsCountItemsWithoutDuplicate()
        {
            var list = Enumerable.Range(1, 10).ToList();
            var sample = RandomKit.NextSample(list, 4);
            Assert.AreEqual(4, sample.Count);
            Assert.AreEqual(4, sample.Distinct().Count());
        }

        [TestMethod]
        public void ShuffleInPlace_PreservesElements()
        {
            var list = Enumerable.Range(1, 10).ToList();
            var original = new List<int>(list);
            RandomKit.ShuffleInPlace(list);
            CollectionAssert.AreEquivalent(original, list);
        }

        [TestMethod]
        public void Shuffle_PreservesElements()
        {
            var source = Enumerable.Range(1, 10).ToList();
            var result = RandomKit.Shuffle(source);
            Assert.AreEqual(source.Count, result.Count());
            CollectionAssert.AreEquivalent(source, result.ToList());
        }

        [TestMethod]
        public void NextWeightedItem_AlwaysReturnsFromSet()
        {
            var items = new List<string> { "a", "b" };
            for (var i = 0; i < 100; i++)
            {
                var item = RandomKit.NextWeightedItem(items, x => x == "a" ? 9 : 1);
                Assert.IsTrue(item is "a" or "b");
            }
        }

        [TestMethod]
        public void Roll_ExtremeValues()
        {
            Assert.IsTrue(RandomKit.Roll(1));
            Assert.IsFalse(RandomKit.Roll(0));
        }

        [TestMethod]
        public void Pick_ReturnsOneOfInputs()
        {
            for (var i = 0; i < 100; i++)
            {
                var p = RandomKit.Pick(1, 2, 3);
                Assert.IsTrue(p is 1 or 2 or 3);
            }
        }

        [TestMethod]
        public void NextBytes_FillBytes_Work()
        {
            var bytes = RandomKit.NextBytes(16);
            Assert.AreEqual(16, bytes.Length);

            var buffer = new byte[32];
            RandomKit.FillBytes(buffer);
            Assert.AreEqual(32, buffer.Length);
        }

        [TestMethod]
        public void NextGaussian_ReturnsFiniteValue()
        {
            var v = RandomKit.NextGaussian(0, 1);
            Assert.IsFalse(double.IsNaN(v));
            Assert.IsFalse(double.IsInfinity(v));
        }

        [TestMethod]
        public void NextInts_ReturnsCountValues()
        {
            var values = RandomKit.NextInts(5, 1, 3);
            Assert.AreEqual(5, values.Count());
            Assert.IsTrue(values.All(v => v >= 1 && v < 3));
        }
    }
}
