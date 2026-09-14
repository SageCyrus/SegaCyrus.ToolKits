using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace SegaCyrus.ToolKits.Model.File
{
    /// <summary>
    /// ZIP树节点，用于以层级方式描述压缩包中的目录结构。
    /// 每个节点要么是文件（有 Name + Data），要么是目录（有 Name + Items）。
    /// </summary>
    [DataContract]
    public class ZipItem
    {
        /// <summary>
        /// 节点名称（文件名或目录名）
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 文件数据，目录节点为 null
        /// </summary>
        public byte[] Data { get; set; }

        /// <summary>
        /// 子节点列表，文件节点为 null
        /// </summary>
        public List<ZipItem> Items { get; set; }

        /// <summary>
        /// 是否为目录节点
        /// </summary>
#if NET6_0_OR_GREATER
        [System.Text.Json.Serialization.JsonIgnore]
#else
        [Newtonsoft.Json.JsonIgnore]
#endif
        public bool IsDirectory => Data == null;

        /// <summary>
        /// 创建空节点
        /// </summary>
        public ZipItem()
        {
        }

        /// <summary>
        /// 创建文件节点
        /// </summary>
        public ZipItem(string name, byte[] data)
        {
            Name = name;
            Data = data;
        }

        /// <summary>
        /// 创建目录节点
        /// </summary>
        public ZipItem(string name, IEnumerable<ZipItem> items)
        {
            Name = name;
            Items = items?.ToList();
        }
    }
}
