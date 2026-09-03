using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using SegaCyrus.ToolKits.Extension.CommonExtension;
using SegaCyrus.ToolKits.Model.File;
using SharpZip = ICSharpCode.SharpZipLib.Zip;

namespace SegaCyrus.ToolKits.Kit.File
{
    /// <summary>
    /// 文件处理工具包
    /// </summary>
    public class FileKit
    {
        #region 文件名处理

        /// <summary>
        /// 过滤不合法的文件路径字符
        /// </summary>
        /// <param name="path">路径</param>
        /// <returns></returns>
        public static string FilterInvalidPathChars(string path)
        {
            AssertKit.AssertNotEmpty(path, nameof(path));
            var titleBuilder = new StringBuilder(path);
            foreach (var rInvalidChar in Path.GetInvalidPathChars())
                titleBuilder = titleBuilder.Replace(rInvalidChar.ToString(), string.Empty);
            return titleBuilder.ToString();
        }

        /// <summary>
        /// 过滤不合法的文件名字符
        /// </summary>
        /// <param name="name">文件名</param>
        /// <returns></returns>
        public static string FilterInvalidFileNameChars(string name)
        {
            AssertKit.AssertNotEmpty(name, nameof(name));
            var titleBuilder = new StringBuilder(name);
            foreach (var rInvalidChar in Path.GetInvalidFileNameChars())
                titleBuilder = titleBuilder.Replace(rInvalidChar.ToString(), string.Empty);
            return titleBuilder.ToString();
        }

        #endregion

        #region ZIP压缩处理

        /// <summary>
        /// 打包扁平条目列表为ZIP
        /// </summary>
        /// <param name="entries">ZIP条目列表</param>
        /// <param name="compressLevel">压缩等级（0-9）</param>
        /// <returns>ZIP文件字节数组</returns>
        public static byte[] PackToZip(IEnumerable<ZipEntry> entries, int compressLevel = 0)
        {
            AssertKit.AssertNotNull(entries, nameof(entries));
            AssertKit.AssertEQGreater(compressLevel, 0, nameof(compressLevel));

            using (var stream = new MemoryStream())
            {
                using (var zipStream = new SharpZip.ZipOutputStream(stream))
                {
                    zipStream.SetLevel(compressLevel);

                    foreach (var entry in entries)
                    {
                        if (entry == null || string.IsNullOrEmpty(entry.Path))
                            continue;

                        var entryPath = FilterInvalidPathChars(entry.Path);
                        var zipEntry = new SharpZip.ZipEntry(entryPath)
                        {
                            DateTime = DateTime.Now
                        };
                        zipStream.PutNextEntry(zipEntry);

                        if (!entry.IsDirectory && entry.Data != null && entry.Data.Length > 0)
                            zipStream.Write(entry.Data, 0, entry.Data.Length);

                        zipStream.CloseEntry();
                    }

                    zipStream.Finish();
                }

                return stream.ToArray();
            }
        }

        /// <summary>
        /// 打包树形结构为ZIP
        /// </summary>
        /// <param name="root">ZIP树根节点，其 Name 会作为所有子条目的路径前缀</param>
        /// <param name="compressLevel">压缩等级（0-9）</param>
        /// <returns>ZIP文件字节数组</returns>
        /// <remarks>
        /// 若不需要外层包装目录，请使用
        /// <see cref="PackToZip(List{ZipItem}, int)"/>
        /// </remarks>
        public static byte[] PackToZip(ZipItem root, int compressLevel = 0)
        {
            AssertKit.AssertNotNull(root, nameof(root));
            AssertKit.AssertEQGreater(compressLevel, 0, nameof(compressLevel));

            return PackToZip(FlattenTree(root, string.Empty), compressLevel);
        }

        /// <summary>
        /// 打包多个树形节点为ZIP，每个节点作为顶层条目，不额外包装目录
        /// </summary>
        /// <param name="roots">ZIP树节点列表，每个节点的 Name 直接作为顶层路径</param>
        /// <param name="compressLevel">压缩等级（0-9）</param>
        /// <returns>ZIP文件字节数组</returns>
        public static byte[] PackToZip(List<ZipItem> roots, int compressLevel = 0)
        {
            AssertKit.AssertNotNull(roots, nameof(roots));
            AssertKit.AssertEQGreater(compressLevel, 0, nameof(compressLevel));

            var entries = new List<ZipEntry>();
            foreach (var root in roots)
            {
                if (root != null)
                    entries.AddRange(FlattenTree(root, string.Empty));
            }

            return PackToZip(entries, compressLevel);
        }

        /// <summary>
        /// 解压ZIP文件为扁平条目列表
        /// </summary>
        /// <param name="zipData">ZIP文件字节数组</param>
        /// <returns>ZIP条目列表</returns>
        public static List<ZipEntry> Unzip(byte[] zipData)
        {
            AssertKit.AssertNotNull(zipData, nameof(zipData));

            var result = new List<ZipEntry>();

            using (var zipStream = new SharpZip.ZipInputStream(new MemoryStream(zipData)))
            {
                while (true)
                {
                    var entry = zipStream.GetNextEntry();
                    if (entry == null)
                        break;

                    var isDir = entry.IsDirectory
                                || (!string.IsNullOrEmpty(entry.Name) && entry.Name.EndsWith("/"));

                    if (isDir)
                    {
                        result.Add(new ZipEntry(entry.Name, null, true));
                    }
                    else
                    {
                        using (var itemStream = new MemoryStream())
                        {
                            var buffer = new byte[4096];
                            int readLen;
                            while ((readLen = zipStream.Read(buffer, 0, buffer.Length)) > 0)
                                itemStream.Write(buffer, 0, readLen);

                            result.Add(new ZipEntry(entry.Name, itemStream.ToArray()));
                        }
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// 解压ZIP文件为树形结构
        /// </summary>
        /// <param name="zipData">ZIP文件字节数组</param>
        /// <returns>ZIP树根节点</returns>
        /// <remarks>
        /// 返回值始终包含一个虚拟根节点包装所有顶层条目。
        /// 若不需要包装层，请使用 <see cref="UnzipToTreeList(byte[])"/>。
        /// </remarks>
        public static ZipItem UnzipToTree(byte[] zipData)
        {
            var entries = Unzip(zipData);
            return BuildTree(entries);
        }

        /// <summary>
        /// 解压ZIP文件为顶层条目列表，不额外包装虚拟根节点
        /// </summary>
        /// <param name="zipData">ZIP文件字节数组</param>
        /// <returns>顶层 ZIP 树节点列表</returns>
        public static List<ZipItem> UnzipToTreeList(byte[] zipData)
        {
            var entries = Unzip(zipData);
            var root = BuildTree(entries);
            return root?.Items ?? new List<ZipItem>();
        }

        #region 私有辅助

        /// <summary>
        /// 将树形结构扁平化为 ZipEntry 列表
        /// </summary>
        private static List<ZipEntry> FlattenTree(ZipItem item, string parentPath)
        {
            var result = new List<ZipEntry>();
            if (item == null)
                return result;

            var currentPath = string.IsNullOrEmpty(parentPath)
                ? item.Name ?? string.Empty
                : $"{parentPath}/{item.Name}";

            if (item.IsDirectory)
            {
                // 根节点不创建自身目录条目，仅用 Name 作为子路径前缀
                if (!string.IsNullOrEmpty(parentPath) && !string.IsNullOrEmpty(currentPath))
                    result.Add(new ZipEntry(currentPath, null, true));

                if (item.Items.NotEmpty())
                    foreach (var child in item.Items)
                        result.AddRange(FlattenTree(child, currentPath));
            }
            else if (!string.IsNullOrEmpty(currentPath))
            {
                result.Add(new ZipEntry(currentPath, item.Data));
            }

            return result;
        }

        /// <summary>
        /// 从扁平 ZipEntry 列表构建树形结构
        /// </summary>
        private static ZipItem BuildTree(IEnumerable<ZipEntry> entries)
        {
            var list = entries?.Where(e => e != null && !string.IsNullOrEmpty(e.Path)).ToList();
            if (list == null || list.Count == 0)
                return null;

            var root = new ZipItem { Items = new List<ZipItem>() };

            foreach (var entry in list)
            {
                var path = entry.Path.TrimEnd('/');
                var parts = path.Split('/');

                var current = root;
                for (var i = 0; i < parts.Length; i++)
                {
                    if (current.Items == null)
                        current.Items = new List<ZipItem>();

                    var isLast = i == parts.Length - 1;

                    if (isLast && !entry.IsDirectory)
                    {
                        // 文件叶子节点
                        current.Items.Add(new ZipItem(parts[i], entry.Data));
                    }
                    else
                    {
                        // 目录节点
                        var child = current.Items.Find(c => c.IsDirectory && c.Name == parts[i]);
                        if (child == null)
                        {
                            child = new ZipItem { Name = parts[i], Items = new List<ZipItem>() };
                            current.Items.Add(child);
                        }

                        current = child;
                    }
                }
            }

            return root;
        }

        #endregion

        #endregion
    }
}