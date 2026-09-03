using Microsoft.VisualStudio.TestTools.UnitTesting;
using SegaCyrus.ToolKits.Kit;

namespace SegaCyrus.ToolKits.MSTest.Kit
{
    /// <summary>
    /// <see cref="RegexKit"/> 单元测试
    /// </summary>
    [TestClass]
    public class RegexKitTests
    {
        [TestMethod]
        public void VarialbeNamedRegeix_IsCompiled_And_NotNull()
        {
            Assert.IsNotNull(RegexKit.VarialbeNamedRegeix);
            Assert.IsTrue(RegexKit.VarialbeNamedRegeix.Options.HasFlag(System.Text.RegularExpressions.RegexOptions.Compiled));
        }

        [TestMethod]
        [DataRow("abc", true)]
        [DataRow("_abc", true)]
        [DataRow("_", true)]
        [DataRow("a1_b2C3", true)]
        [DataRow("A", true)]
        [DataRow("1abc", false)]   // 不能以数字开头
        [DataRow("", false)]       // 空
        [DataRow("abc def", false)]// 空格
        [DataRow("a-b", false)]    // 连字符
        [DataRow("a.b", false)]    // 点
        [DataRow("你好", false)]    // 中文
        public void CheckVariableNameSpecification_VariousNames(string variableName, bool expected)
        {
            Assert.AreEqual(expected, RegexKit.CheckVariableNameSpecification(variableName));
        }

        [TestMethod]
        public void CheckVariableNameSpecification_Null_Throws()
        {
            Assert.ThrowsExactly<System.ArgumentNullException>(() => RegexKit.CheckVariableNameSpecification(null));
        }
    }
}
