using System.IO;

namespace SegaCyrus.ToolKits.Kit.File
{
    /// <summary>
    /// 大文件读取
    /// </summary>
    public class BigFile
    {
        #region InternalProperty

        internal Stream FileStreamHandle { get; set; }
        internal bool EOF { get; set; }
        internal int BlockSize { get; set; }

        #endregion

        #region Construct Function

        /// <summary>
        /// 大文件处理
        /// </summary>
        /// <param name="fileName">文件路径</param>
        public BigFile(string fileName)
        {
            FileStreamHandle = new FileStream(fileName, FileMode.Open, FileAccess.Read);
        }

        /// <summary>
        /// 大文件处理
        /// </summary>
        /// <param name="stream">文件流</param>
        public BigFile(Stream stream)
        {
            FileStreamHandle = AssertKit.AssertNotNull(stream, nameof(stream));
        }

        #endregion

        /// <summary>
        /// 按指定偏移量读取数据
        /// </summary>
        /// <param name="target">读取到</param>
        /// <param name="offsetIndex">起始</param>
        /// <param name="lenght">读取大小</param>
        /// <returns>实际读取量</returns>
        public int ReadOffset(ref byte[] target, long offsetIndex, int lenght)
        {
            FileStreamHandle.Seek(offsetIndex, SeekOrigin.Begin);
            var size = FileStreamHandle.Read(target, 0, lenght);
            EOF = size < lenght;
            return size;
        }

        /// <summary>
        /// 按指定偏移量读取数据
        /// </summary>
        /// <param name="offsetIndex">起始</param>
        /// <param name="lenght">读取大小</param>
        /// <param name="size">读取了多少</param>
        /// <returns>返回值</returns>
        public byte[] ReadOffset(long offsetIndex, int lenght, out int size)
        {
            var bytes = new byte[lenght];
            FileStreamHandle.Seek(offsetIndex, SeekOrigin.Begin);
            size = FileStreamHandle.Read(bytes, 0, lenght);
            EOF = size < lenght;
            return bytes;
        }
    }
}