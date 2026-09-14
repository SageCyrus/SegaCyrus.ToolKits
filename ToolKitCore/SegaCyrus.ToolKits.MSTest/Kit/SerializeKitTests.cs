using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SegaCyrus.ToolKits.Kit;

namespace SegaCyrus.ToolKits.MSTest.Kit
{
    [DataContract]
    public class DataContractSample
    {
        [DataMember(Order = 1)]
        public int Id { get; set; }

        [DataMember(Order = 2)]
        public string Name { get; set; }

        [DataMember(Order = 3)]
        public List<int> Values { get; set; } = new List<int>();
    }

    public class XmlSample
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    /// <summary>
    /// <see cref="SerializeKit"/> 单元测试
    /// </summary>
    [TestClass]
    public class SerializeKitTests
    {
        [TestMethod]
        public void GetJSON_SerializesPublicProperties()
        {
            var json = SerializeKit.GetJSON(new XmlSample { Id = 1, Name = "a" });
            StringAssert.Contains(json, "\"Id\":1");
            StringAssert.Contains(json, "\"Name\":\"a\"");
        }

        [TestMethod]
        public void GetJSON_Null_ReturnsJsonNull()
        {
            Assert.AreEqual("null", SerializeKit.GetJSON<XmlSample>(null));
        }

        [TestMethod]
        public void DeserializeJSON_WithGeneric_ReturnsObject()
        {
            var obj = SerializeKit.DeserializeJSON<XmlSample>("{\"Id\":2,\"Name\":\"b\"}");
            Assert.AreEqual(2, obj.Id);
            Assert.AreEqual("b", obj.Name);
        }

        [TestMethod]
        public void DeserializeJSON_WithType_ReturnsObject()
        {
            var obj = (XmlSample)SerializeKit.DeserializeJSON("{\"Id\":3,\"Name\":\"c\"}", typeof(XmlSample));
            Assert.AreEqual(3, obj.Id);
            Assert.AreEqual("c", obj.Name);
        }

        [TestMethod]
        public void GetJSON_DeserializeJSON_RoundTrip_Chinese()
        {
            const string text = "中文内容 & <tag> \"quote\" 'single'";
            var sample = new XmlSample { Id = 8, Name = text };
            var json = SerializeKit.GetJSON(sample);
            var restored = SerializeKit.DeserializeJSON<XmlSample>(json);
            Assert.AreEqual(sample.Id, restored.Id);
            Assert.AreEqual(sample.Name, restored.Name);
        }

        [TestMethod]
        public void GetFormatJSON_ContainsIndentNewLine()
        {
            var json = SerializeKit.GetFormatJSON(new XmlSample { Id = 1, Name = "a" });
            StringAssert.Contains(json, Environment.NewLine);
        }

        [TestMethod]
        public void GetFormatJSON_Null_ReturnsEmptyString()
        {
            Assert.AreEqual(string.Empty, SerializeKit.GetFormatJSON<XmlSample>(null));
        }

        #region XML

        [TestMethod]
        public void GetXML_DeserializeXML_RoundTrip()
        {
            var sample = new XmlSample { Id = 1, Name = "x" };
            var xml = SerializeKit.GetXML(sample);
            StringAssert.Contains(xml, "<Id>1</Id>");
            var restored = SerializeKit.DeserializeXML<XmlSample>(xml);
            Assert.AreEqual(1, restored.Id);
            Assert.AreEqual("x", restored.Name);
        }

        [TestMethod]
        public void GetFormatXML_ContainsIndent()
        {
            var xml = SerializeKit.GetFormatXML(new XmlSample { Id = 1, Name = "x" });
            StringAssert.Contains(xml, Environment.NewLine);
        }

        [TestMethod]
        public void DeserializeXML_WithType_ReturnsObject()
        {
            var xml = SerializeKit.GetXML(new XmlSample { Id = 2, Name = "y" });
            var obj = (XmlSample)SerializeKit.DeserializeXML(xml, typeof(XmlSample));
            Assert.AreEqual(2, obj.Id);
        }

        #endregion

    }
}
