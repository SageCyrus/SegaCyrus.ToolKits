using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace SegaCyrus.ToolKits.Model.File
{
    /// <summary>
    /// ZIP条目（扁平结构），表示压缩包中的单个文件或目录
    /// </summary>
    [DataContract]
    public class ZipEntry
    {
        /// <summary>
        /// 压缩包内的完整路径，如 "docs/report.pdf"
        /// </summary>
        [DataMember, JsonProperty]
        public string Path { get; set; }

        /// <summary>
        /// 文件数据，目录时为 null
        /// </summary>
        [DataMember, JsonProperty]
        public byte[] Data { get; set; }

        /// <summary>
        /// 是否为目录
        /// </summary>
        [DataMember, JsonProperty]
        public bool IsDirectory { get; set; }

        /// <summary>
        /// 创建空 ZIP 条目
        /// </summary>
        public ZipEntry()
        {
        }

        /// <summary>
        /// 创建 ZIP 条目
        /// </summary>
        /// <param name="path">压缩包内完整路径</param>
        /// <param name="data">文件数据</param>
        /// <param name="isDirectory">是否为目录</param>
        public ZipEntry(string path, byte[] data, bool isDirectory = false)
        {
            Path = path;
            Data = data;
            IsDirectory = isDirectory;
        }
    }
}
