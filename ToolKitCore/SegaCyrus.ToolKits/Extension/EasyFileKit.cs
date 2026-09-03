using System.Collections.Generic;
using System.Linq;
using SegaCyrus.ToolKits.Extension.CodeExtension;
using SegaCyrus.ToolKits.Extension.CommonExtension;
using SegaCyrus.ToolKits.Kit.File;
using SegaCyrus.ToolKits.Model.Common;
using SegaCyrus.ToolKits.Model.File;

namespace SegaCyrus.ToolKits.Extension.FileExtension
{
    /// <summary>
    /// 文件扩展包
    /// </summary>
    public static class EasyFileKit
    {
        /// <summary>
        /// 打包ZIP文件
        /// </summary>
        /// <param name="fileData">文件数据，Key 为文件名，Value 为文件数据</param>
        /// <param name="compareLevel">压缩级别</param>
        /// <returns>压缩文件的二进制流</returns>
        /// <remarks>
        /// <see cref="FileKit.PackToZip(ZipItem, int)"/>
        /// </remarks>
        public static byte[] PackToZip(this Dictionary<string, byte[]> fileData, int compareLevel = 0)
        {
            if (fileData.IsEmpty())
                return null;

            return FileKit.PackToZip(
                fileData.Select(c => new ZipEntry(c.Key, c.Value)),
                compareLevel);
        }

        /// <summary>
        /// 打包ZIP文件
        /// </summary>
        /// <param name="fileData">文件数据，Key 为文件名，Value 为文件内容</param>
        /// <param name="compareLevel">压缩级别</param>
        /// <returns>压缩文件的二进制流</returns>
        /// <remarks>
        /// <see cref="FileKit.PackToZip(ZipItem, int)"/>
        /// </remarks>
        public static byte[] PackToZip(this Dictionary<string, string> fileData, int compareLevel = 0)
        {
            if (fileData.IsEmpty())
                return null;

            return FileKit.PackToZip(
                fileData.Select(c => new ZipEntry(c.Key, c.Value.ToUTF8())),
                compareLevel);
        }

        /// <summary>
        /// 解压缩文件
        /// </summary>
        /// <param name="zipFile">压缩文件数据</param>
        /// <returns>解压结果，Key 为文件路径，Value 为文件数据</returns>
        /// <remarks>
        /// <see cref="FileKit.Unzip(byte[])"/>
        /// </remarks>
        public static List<KVModel<string, byte[]>> UnPackZipToBytes(this byte[] zipFile)
        {
            var entries = FileKit.Unzip(zipFile) ?? new List<ZipEntry>();
            return entries
                .Where(e => !e.IsDirectory)
                .Select(e => new KVModel<string, byte[]>(e.Path, e.Data))
                .ToList();
        }

        /// <summary>
        /// 解压缩文件
        /// </summary>
        /// <param name="zipFile">压缩文件数据</param>
        /// <returns>解压结果，Key 为文件路径，Value 为文件内容（UTF8字符串）</returns>
        /// <remarks>
        /// <see cref="FileKit.Unzip(byte[])"/>
        /// </remarks>
        public static List<KVModel<string, string>> UnPackZipToString(this byte[] zipFile)
        {
            var entries = FileKit.Unzip(zipFile) ?? new List<ZipEntry>();
            return entries
                .Where(e => !e.IsDirectory)
                .Select(e => new KVModel<string, string>(e.Path, e.Data?.UTF8ToString()))
                .ToList();
        }
        
    }
}
