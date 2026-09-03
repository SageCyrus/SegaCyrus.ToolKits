#pragma warning disable CS0618 // 类型或成员已过时
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SegaCyrus.ToolKits.Kit.Labs;
using SegaCyrus.ToolKits.Model.Labs;
using SegaCyrus.ToolKits.Model.Labs.Enums;
using Newtonsoft.Json.Linq;

namespace SegaCyrus.ToolKits.MSTest.Kit
{
    public class LabCompareModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double Score { get; set; }
        public bool Enabled { get; set; }
    }

    /// <summary>
    /// <see cref="LabsKit"/> 单元测试
    /// </summary>
    [TestClass]
    public class LabsKitTests
    {
        [TestMethod]
        public void CompareModelByJson_EqualModels_NoDifference()
        {
            var a = new LabCompareModel { Id = 1, Name = "x", Score = 1.5, Enabled = true };
            var b = new LabCompareModel { Id = 1, Name = "x", Score = 1.5, Enabled = true };
            var result = LabsKit.CompareModelByJson(a, b);
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.DifferenceResult);
            Assert.IsEmpty(result.DifferenceResult);
        }

        [TestMethod]
        public void CompareModelByJson_ValueDifferent_ReportsDifference()
        {
            var a = new LabCompareModel { Id = 1, Name = "x", Score = 1.5, Enabled = true };
            var b = new LabCompareModel { Id = 1, Name = "y", Score = 1.5, Enabled = true };
            var result = LabsKit.CompareModelByJson(a, b);
            Assert.IsTrue(result.DifferenceResult.Count > 0);
            Assert.IsTrue(result.DifferenceResult.Any(c => c.DifferenceType == DifferenceTypeEnum.ValueDifference));
        }

        [TestMethod]
        public void CompareJson_WithDifferentTokens_NoThrow()
        {
            var source = JObject.Parse(@"{""a"":1,""b"":""2"",""c"":true}");
            var target = JObject.Parse(@"{""a"":1,""b"":""2"",""c"":false}");
            var result = LabsKit.CompareJson(source, target);
            Assert.IsNotNull(result);
            Assert.IsNotEmpty(result.DifferenceResult);
        }

        [TestMethod]
        public void JsonDifferenceModelResult_ConstructorsAndAppendWork()
        {
            var single = new JsonDifferenceModelResult(
                new JsonDifferenceItemModelResult("x", DifferenceTypeEnum.None));
            Assert.AreEqual(1, single.DifferenceResult.Count);

            var appended = new JsonDifferenceModelResult().Append(
                new JsonDifferenceItemModelResult("p", DifferenceTypeEnum.SourceNotExist, "1", "2", "info"));
            Assert.AreEqual(1, appended.DifferenceResult.Count);
            Assert.AreEqual("p", appended.DifferenceResult[0].Path);
            Assert.AreEqual(DifferenceTypeEnum.SourceNotExist, appended.DifferenceResult[0].DifferenceType);
            Assert.AreEqual("info", appended.DifferenceResult[0].MoreInformation);
        }
    }
}
