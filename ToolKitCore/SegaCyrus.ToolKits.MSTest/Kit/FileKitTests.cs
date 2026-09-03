using System;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SegaCyrus.ToolKits.Kit.File;
using SegaCyrus.ToolKits.Model.File;

namespace SegaCyrus.ToolKits.MSTest.Kit
{
    /// <summary>
    /// <see cref="FileKit"/> 单元测试
    /// </summary>
    [TestClass]
    public class FileKitTests
    {
        [TestMethod]
        public void FilterInvalidPathChars_RemovesInvalidChars()
        {
            const string input = @"C:\temp\valid\a<b|c>d""e" + "\0";
            var filtered = FileKit.FilterInvalidPathChars(input);
            var invalid = Path.GetInvalidPathChars();
            Assert.IsFalse(filtered.Any(c => invalid.Contains(c)));
            Assert.IsTrue(filtered.Contains(@"C:\temp\valid\a"));
        }

        [TestMethod]
        public void FilterInvalidFileNameChars_RemovesInvalidChars()
        {
            const string input = @"a<b>:c|d?e*f""g\h\i" + "\0";
            var filtered = FileKit.FilterInvalidFileNameChars(input);
            var invalid = Path.GetInvalidFileNameChars();
            Assert.IsFalse(filtered.Any(c => invalid.Contains(c)));
            Assert.AreEqual("abcdefghi", filtered);
        }


        [TestMethod]
        public void PackToZip_Unzip_RoundTrip_FlatEntries()
        {
            var data = Encoding.UTF8.GetBytes("file content");
            var zip = FileKit.PackToZip(new[]
            {
                new ZipEntry("a.txt", data)
            });
            Assert.IsNotNull(zip);
            Assert.IsNotEmpty(zip);

            var entries = FileKit.Unzip(zip);
            Assert.AreEqual(1, entries.Count);
            Assert.AreEqual("a.txt", entries[0].Path);
            Assert.IsFalse(entries[0].IsDirectory);
            CollectionAssert.AreEqual(data, entries[0].Data);
        }

        [TestMethod]
        public void PackToZip_Unzip_DirectoryAndNestedFile_RoundTrip()
        {
            var zip = FileKit.PackToZip(new[]
            {
                new ZipEntry("folder/", null, isDirectory: true),
                new ZipEntry("folder/inner.txt", Encoding.UTF8.GetBytes("inner"))
            });
            var entries = FileKit.Unzip(zip);
            Assert.AreEqual(2, entries.Count);
            var inner = entries.First(e => e.Path.Contains("inner.txt"));
            Assert.AreEqual("inner", Encoding.UTF8.GetString(inner.Data));
        }

        [TestMethod]
        public void PackToZip_ZipItemTree_RoundTrip()
        {
            var root = new ZipItem("root",
                new System.Collections.Generic.List<ZipItem>
                {
                    new ZipItem("folder", new System.Collections.Generic.List<ZipItem>
                    {
                        new ZipItem("f.txt", Encoding.UTF8.GetBytes("hello tree"))
                    }),
                    new ZipItem("single.bin", new byte[] { 1, 2 })
                });
            var zip = FileKit.PackToZip(root);
            Assert.IsNotNull(zip);

            var entries = FileKit.Unzip(zip);
            var tree = FileKit.UnzipToTreeList(zip);
            Assert.IsNotNull(tree);
            Assert.IsTrue(entries.Count >= 2);
            var textEntry = entries.FirstOrDefault(e => e.Path.EndsWith("f.txt", StringComparison.OrdinalIgnoreCase));
            Assert.IsNotNull(textEntry);
            Assert.AreEqual("hello tree", Encoding.UTF8.GetString(textEntry.Data));
        }

        [TestMethod]
        public void PackToZip_EmptyEntries_ReturnsEmptyZip()
        {
            var zip = FileKit.PackToZip(Array.Empty<ZipEntry>());
            Assert.IsNotNull(zip);
            Assert.IsEmpty(FileKit.Unzip(zip));
        }

        [TestMethod]
        public void Unzip_InvalidData_Throws()
        {
            Assert.Throws<Exception>(() => FileKit.Unzip(new byte[] { 1, 2, 3, 4, 5 }));
        }
    }
}
