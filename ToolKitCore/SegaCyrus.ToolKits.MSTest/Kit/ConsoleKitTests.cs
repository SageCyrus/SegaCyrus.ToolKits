using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SegaCyrus.ToolKits.Kit;
using SegaCyrus.ToolKits.Model.Console.Attributes;

namespace SegaCyrus.ToolKits.MSTest.Kit
{
    /// <summary>
    /// 命令行参数模型
    /// </summary>
    public class NoCommandPropertyModel
    {
        public string X { get; set; }
    }

    /// <summary>
    /// 命令行参数模型
    /// </summary>
    public class CommandArgsTestModel
    {
        [CommandProperty("a", "alpha", "名称", false, true)]
        public string Alpha { get; set; }

        [CommandProperty("c", "count")]
        public int Count { get; set; }

        [CommandProperty("b")]
        public bool Enable { get; set; }

        [CommandProperty("n", "num")]
        public double Num { get; set; }

        [CommandProperty("tag", "tags")]
        public List<string> Tags { get; set; }

        [CommandProperty("m", "multi")]
        public int[] Multi { get; set; }

        [CommandProperty("u", "username", "用户名", false, true)]
        public string Username { get; set; }
    }

    /// <summary>
    /// <see cref="ConsoleKit"/> 单元测试
    /// </summary>
    [TestClass]
    public class ConsoleKitTests
    {
        #region WriteConsole

        private static string CaptureConsole(Action action)
        {
            var old = Console.Out;
            using var writer = new StringWriter();
            Console.SetOut(writer);
            try
            {
                action();
            }
            finally
            {
                Console.SetOut(old);
            }
            return writer.ToString();
        }

        [TestMethod]
        public void WriteConsole_WithTime_WritesTimePrefix()
        {
            var output = CaptureConsole(() => ConsoleKit.WriteConsole("hello console"));
            StringAssert.Contains(output, "hello console");
            Assert.IsTrue(output.StartsWith("20"), "期望输出包含时间前缀,实际: " + output);
        }

        [TestMethod]
        public void WriteConsole_WithoutTime_NoTimePrefix()
        {
            var output = CaptureConsole(() => ConsoleKit.WriteConsole("plain", ConsoleColor.Green, withTime: false));
            Assert.AreEqual("plain" + Environment.NewLine, output);
        }

        [TestMethod]
        public void WriteConsole_ColorDoesNotLeakForegroundColor()
        {
            var oldColor = Console.ForegroundColor;
            CaptureConsole(() => ConsoleKit.WriteConsole("color-leak-check", ConsoleColor.Yellow, withTime: false));
            Assert.AreEqual(oldColor, Console.ForegroundColor);
        }

        #endregion

        #region WaitInput / WaitConfirmInput

        private static void WithConsoleIn(string input, Action action)
        {
            var oldIn = Console.In;
            var oldOut = Console.Out;
            using var reader = new StringReader(input);
            using var writer = new StringWriter();
            Console.SetIn(reader);
            Console.SetOut(writer);
            try
            {
                action();
            }
            finally
            {
                Console.SetIn(oldIn);
                Console.SetOut(oldOut);
            }
        }

        [TestMethod]
        public void WaitInput_ReadsNonEmptyInput()
        {
            WithConsoleIn("hello" + Environment.NewLine, () =>
            {
                var result = ConsoleKit.WaitInput("请输入内容", withTime: false);
                Assert.AreEqual("hello", result);
            });
        }

        [TestMethod]
        public void WaitInput_EmptyThenValid_Retries()
        {
            WithConsoleIn(Environment.NewLine + "second" + Environment.NewLine, () =>
            {
                var result = ConsoleKit.WaitInput("请输入内容", withTime: false);
                Assert.AreEqual("second", result);
            });
        }

        [TestMethod]
        public void WaitConfirmInput_ExactMatch_Returns()
        {
            WithConsoleIn("yes" + Environment.NewLine, () =>
            {
                ConsoleKit.WaitConfirmInput("yes", withTime: false);
            });
        }

        #endregion

        #region ParseArgs

        [TestMethod]
        public void ParseArgs_ShortNameAndValues()
        {
            var result = ConsoleKit.ParseArgs<CommandArgsTestModel>(
                new[] { "-a", "abc", "-c", "3", "-u", "tester" });
            Assert.IsNotNull(result);
            Assert.AreEqual("abc", result.Alpha);
            Assert.AreEqual(3, result.Count);
            Assert.AreEqual("tester", result.Username);
            Assert.IsFalse(result.Enable);
        }

        [TestMethod]
        public void ParseArgs_BoolFlag_OnlySetsTrue()
        {
            var result = ConsoleKit.ParseArgs<CommandArgsTestModel>(
                new[] { "-a", "x", "-u", "tester", "-b" });
            Assert.IsTrue(result.Enable);
        }

        [TestMethod]
        public void ParseArgs_FullName_Works()
        {
            var result = ConsoleKit.ParseArgs<CommandArgsTestModel>(
                new[] { "-alpha", "abc", "-username", "tester" });
            Assert.AreEqual("abc", result.Alpha);
            Assert.AreEqual("tester", result.Username);
        }

        [TestMethod]
        public void ParseArgs_ListProperty_CollectsValuesUntilNextFlag()
        {
            var result = ConsoleKit.ParseArgs<CommandArgsTestModel>(
                new[] { "-a", "x", "-u", "tester", "-tag", "t1", "t2", "t3", "-c", "5" });
            Assert.IsNotNull(result.Tags);
            Assert.AreEqual(3, result.Tags.Count);
            CollectionAssert.AreEqual(new[] { "t1", "t2", "t3" }, result.Tags);
            Assert.AreEqual(5, result.Count);
        }

        [TestMethod]
        public void ParseArgs_ArrayProperty_Works()
        {
            var result = ConsoleKit.ParseArgs<CommandArgsTestModel>(
                new[] { "-a", "x", "-u", "tester", "-m", "1", "2", "3" });
            CollectionAssert.AreEqual(new[] { 1, 2, 3 }, result.Multi);
        }

        [TestMethod]
        public void ParseArgs_DoubleProperty_Works()
        {
            var result = ConsoleKit.ParseArgs<CommandArgsTestModel>(
                new[] { "-a", "x", "-u", "tester", "-n", "3.5" });
            Assert.AreEqual(3.5, result.Num);
        }

        [TestMethod]
        public void ParseArgs_MissingRequired_Throws()
        {
            var ex = Assert.ThrowsExactly<ArgumentException>(
                () => ConsoleKit.ParseArgs<CommandArgsTestModel>(new[] { "-a", "abc" }));
            StringAssert.Contains(ex.Message, "参数必填:");
            StringAssert.Contains(ex.Message, "Username");
        }

        [TestMethod]
        public void ParseArgs_MissingRequired_WithCheckRequireFalse_DoesNotThrow()
        {
            var result = ConsoleKit.ParseArgs<CommandArgsTestModel>(
                new[] { "-a", "abc" }, checkRequire: false);
            Assert.IsNotNull(result);
            Assert.AreEqual("abc", result.Alpha);
            Assert.IsNull(result.Username);
        }

        [TestMethod]
        public void ParseArgs_HelpFlag_WritesHelpContentAndReturnsDefault()
        {
            var output = CaptureConsole(() =>
            {
                var result = ConsoleKit.ParseArgs<CommandArgsTestModel>(
                    new[] { "-h" }, helpContent: "自定义帮助内容");
                Assert.IsNull(result);
            });
            StringAssert.Contains(output, "自定义帮助内容");
        }

        [TestMethod]
        public void ParseArgs_UnknownCommand_Throws()
        {
            Assert.ThrowsExactly<ArgumentException>(
                () => ConsoleKit.ParseArgs<CommandArgsTestModel>(new[] { "plain-text" }));
        }

        [TestMethod]
        public void ParseArgs_NoCommandProperty_ReturnsInstanceWithoutMapping()
        {
            // 没有任何 CommandProperty 时,未知参数被忽略,返回默认实例
            var result = ConsoleKit.ParseArgs<NoCommandPropertyModel>(new[] { "-whatever" });
            Assert.IsNotNull(result);
            Assert.IsNull(result.X);
        }

        #endregion
    }
}
