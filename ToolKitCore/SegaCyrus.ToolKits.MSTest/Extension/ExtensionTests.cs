using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SegaCyrus.ToolKits.Extension.CodeExtension;
using SegaCyrus.ToolKits.Extension.CollectionsExtension;
using SegaCyrus.ToolKits.Extension.CommonExtension;
using SegaCyrus.ToolKits.Extension.FileExtension;
using SegaCyrus.ToolKits.Extension.InvokeExtension;
using SegaCyrus.ToolKits.Extension.SerializeExtension;
using SegaCyrus.ToolKits.Model.Common;
using SegaCyrus.ToolKits.MSTest.Kit;

namespace SegaCyrus.ToolKits.MSTest.Extension
{
    /// <summary>
    /// Extension 扩展方法 单元测试
    /// </summary>
    [TestClass]
    public class ExtensionTests
    {
        #region EasyCodeKit (CodeExtension)

        [TestMethod]
        public void CharsToString_Works()
        {
            char[] chars = { 'a', '中', 'b' };
            Assert.AreEqual("a中b", chars.CharsToString());
        }

        [TestMethod]
        public void ToUTF8_UTF8ToString_RoundTrip()
        {
            var text = "扩展方法测试";
            var bytes = text.ToUTF8();
            Assert.AreEqual(text, bytes.UTF8ToString());
        }

        [TestMethod]
        public void Base64_StringExtensions_RoundTrip()
        {
            var base64 = "abc中文".ToBase64();
            Assert.AreEqual("abc中文", base64.Bas64ToString());
        }

        [TestMethod]
        public void Base64_BytesExtensions_RoundTrip()
        {
            byte[] data = { 0, 1, 2, 253, 254, 255 };
            var base64 = data.ToBase64();
            CollectionAssert.AreEqual(data, base64.Base64ToBytes());
        }

        [TestMethod]
        public void MD5_SHA256_Extensions_Work()
        {
            Assert.AreEqual("900150983cd24fb0d6963f7d28e17f72", "abc".MD5());
            Assert.AreEqual("ba7816bf8f01cfea414140de5dae2223b00361a396177a9cb410ff61f20015ad", "abc".SHA256());
            Assert.AreEqual("abc".MD5(), "abc".ToUTF8().MD5());
        }

        [TestMethod]
        public void AESEncrypt_Extensions_RoundTrip()
        {
            var source = "加密内容".ToUTF8();
            var encrypted = source.AESEncrypt();
            CollectionAssert.AreEqual(source, encrypted.AESDecrypt());
        }

        [TestMethod]
        public void RemoveUnescape_Extension_Works()
        {
            Assert.AreEqual("a\tb", @"a\tb".RemoveUnescape());
        }

        #endregion

        #region EasyCollectionsKit (CollectionsExtension)

        [TestMethod]
        public void MergeDictionary_BasicMerge()
        {
            var src = new Dictionary<string, int> { ["a"] = 1, ["b"] = 2 };
            var r = new Dictionary<string, int> { ["c"] = 3 };
            var merged = src.MergeDictionary(r);
            Assert.AreEqual(3, merged.Count);
            Assert.AreEqual(1, merged["a"]);
            Assert.AreEqual(3, merged["c"]);
        }

        [TestMethod]
        public void MergeDictionary_ConflictKeepsSourceByDefault()
        {
            var src = new Dictionary<string, int> { ["k"] = 1 };
            var r = new Dictionary<string, int> { ["k"] = 100 };
            var merged = src.MergeDictionary(r);
            Assert.AreEqual(1, merged["k"]);
        }

        [TestMethod]
        public void MergeDictionary_CustomConflictSelector()
        {
            var src = new Dictionary<string, int> { ["k"] = 1 };
            var r = new Dictionary<string, int> { ["k"] = 100 };
            var merged = src.MergeDictionary(r, (oldValue, newValue) => newValue);
            Assert.AreEqual(100, merged["k"]);
        }

        [TestMethod]
        public void MergeDictionary_NullSourceOrRight_NoThrow()
        {
            var dict = new Dictionary<string, int> { ["a"] = 1 };
            Assert.AreEqual(1, ((Dictionary<string, int>)null).MergeDictionary(dict)["a"]);
            Assert.AreEqual(1, dict.MergeDictionary(null)["a"]);
        }

        #endregion

        #region EasyCommonKit (CommonExtension)

        [TestMethod]
        public void ToStream_Bytes_ReturnsStream()
        {
            var bytes = new byte[] { 1, 2, 3, 4 };
            using var stream = bytes.ToStream();
            Assert.IsNotNull(stream);
            Assert.AreEqual(0, stream.Position);
            Assert.AreEqual(4, stream.Length);
        }

        [TestMethod]
        public void ToStream_Null_ReturnsNull()
        {
            Assert.IsNull(((byte[])null).ToStream());
        }

        [TestMethod]
        public void Partition_SplitsAndInvokesAction()
        {
            var source = Enumerable.Range(1, 10).ToList();
            var chunks = new List<IEnumerable<int>>();
            source.Partition(3, chunk => chunks.Add(chunk));
            Assert.AreEqual(4, chunks.Count);
            Assert.AreEqual(3, chunks[0].Count());
            Assert.AreEqual(1, chunks[3].Count());
            Assert.AreEqual(10, chunks.Sum(c => c.Count()));
        }

        [TestMethod]
        public void Partition_NullSource_NoThrow()
        {
            List<int> source = null;
            var called = false;
            source.Partition(3, c => called = true);
            Assert.IsFalse(called);
        }

        [TestMethod]
        public void NotEmpty_IsEmpty_Tests()
        {
            Assert.IsTrue(new[] { 1, 2 }.NotEmpty());
            Assert.IsFalse(new int[0].NotEmpty());
            Assert.IsFalse(((int[])null).NotEmpty());
            Assert.IsTrue(((int[])null).IsEmpty());
            Assert.IsTrue(new int[0].IsEmpty());
            Assert.IsFalse(new[] { 1 }.IsEmpty());
        }

        [TestMethod]
        public void ToHashSet_Deduplicates()
        {
            var set = new[] { 1, 2, 2, 3, 3, 3 }.ToHashSet();
            Assert.AreEqual(3, set.Count);
        }

        [TestMethod]
        public void ToHashSet_NullSource_Throws()
        {
            Assert.ThrowsExactly<ArgumentNullException>(() => ((IEnumerable<int>)null).ToHashSet());
        }

        #endregion

        #region EasyFileKit (FileExtension)

        [TestMethod]
        public void PackToZip_UnPackZipToString_RoundTrip()
        {
            var files = new Dictionary<string, string>
            {
                ["a.txt"] = "AAA",
                ["sub/b.txt"] = "中文内容"
            };
            var zip = files.PackToZip();
            Assert.IsNotNull(zip);
            Assert.IsNotEmpty(zip);

            var unpacked = zip.UnPackZipToString();
            Assert.AreEqual(2, unpacked.Count);
            Assert.AreEqual("AAA", unpacked.First(c => c.Key == "a.txt").Value);
            Assert.AreEqual("中文内容", unpacked.First(c => c.Key == "sub/b.txt").Value);
        }

        [TestMethod]
        public void PackToZip_BytesRoundTrip()
        {
            var files = new Dictionary<string, byte[]> { ["data.bin"] = new byte[] { 1, 2, 3, 255 } };
            var zip = files.PackToZip();
            var unpacked = zip.UnPackZipToBytes();
            Assert.AreEqual(1, unpacked.Count);
            CollectionAssert.AreEqual(new byte[] { 1, 2, 3, 255 }, unpacked[0].Value);
        }

        [TestMethod]
        public void PackToZip_EmptyDictionary_ReturnsNull()
        {
            Assert.IsNull(new Dictionary<string, string>().PackToZip());
            Assert.IsNull(new Dictionary<string, byte[]>().PackToZip());
        }

        #endregion

        #region EasySerializeKit (SerializeExtension)

        [TestMethod]
        public void SerializeExtension_RoundTrips()
        {
            var sample = new XmlSample { Id = 7, Name = "扩展序列化" };

            var json = sample.GetFormatJSON();
            StringAssert.Contains(json, "扩展序列化");
            var fromJson = json.DesializeJSON<XmlSample>();
            Assert.AreEqual(7, fromJson.Id);
            Assert.AreEqual("扩展序列化", fromJson.Name);

            var xml = sample.GetFormatXML();
            var fromXml = xml.DesializeXML<XmlSample>();
            Assert.AreEqual(7, fromXml.Id);
            Assert.AreEqual("扩展序列化", fromXml.Name);
        }

        #endregion

        #region EasyInvokeKit (InvokeExtension)

        [TestMethod]
        public void InvokeExtension_InstanceMember_Works()
        {
            var sample = new TestModels.ReflectSample { Name = "n", Number = 5 };
            Assert.AreEqual("n", sample.GetProperty(nameof(TestModels.ReflectSample.Name)));
            sample.SetProperty(nameof(TestModels.ReflectSample.Number), 9);
            Assert.AreEqual(9, sample.Number);

            sample.SetField(nameof(TestModels.ReflectSample.InstanceField), 123);
            Assert.AreEqual(123, sample.GetField(nameof(TestModels.ReflectSample.InstanceField)));
            Assert.AreEqual(5, sample.CallMethod(nameof(TestModels.ReflectSample.Add), 2, 3));
        }

        [TestMethod]
        public void InvokeExtension_StaticMember_Works()
        {
            var sample = new TestModels.ReflectSample();
            sample.SetStaticProperty(nameof(TestModels.ReflectSample.StaticTextProperty), "static-v");
            Assert.AreEqual("static-v", sample.GetStaticProperty(nameof(TestModels.ReflectSample.StaticTextProperty)));

            sample.SetStaticField(nameof(TestModels.ReflectSample.StaticField), 10);
            Assert.AreEqual(10, sample.GetStaticField(nameof(TestModels.ReflectSample.StaticField)));

            var sample2 = new TestModels.ReflectSample();
            Assert.AreEqual(3, sample2.CallStaticMethod(nameof(TestModels.ReflectSample.StaticAdd), 1, 2));
        }

        //[TestMethod]
        //public void InvokeExtension_TypeStaticMethod_Works()
        //{
        //    var result = typeof(TestModels.ReflectSample).CallStaticMethod(nameof(TestModels.ReflectSample.StaticEcho), "x");
        //    Assert.AreEqual("x", result);
        //}

        #endregion
    }
}
