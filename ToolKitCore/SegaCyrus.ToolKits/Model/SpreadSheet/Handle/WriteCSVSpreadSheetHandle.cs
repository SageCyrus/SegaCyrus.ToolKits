using System.IO;
using System.Text;
using SegaCyrus.ToolKits.Extension.CodeExtension;
using SegaCyrus.ToolKits.Kit;
using SegaCyrus.ToolKits.Model.SpreadSheet.Append;
using SegaCyrus.ToolKits.Model.SpreadSheet.Enums;

namespace SegaCyrus.ToolKits.Model.SpreadSheet.Handle
{
    /// <summary>
    /// CSV写入句柄
    /// </summary>
    public class WriteCSVSpreadSheetHandle : WriteSpreadSheetHandle
    {
        /// <summary>
        /// CSV写入构造器 此方式将使用StringBuilder构造,数据将被写入StringBuilder
        /// </summary>
        public WriteCSVSpreadSheetHandle() : base(SpreadSheetTypeEnum.CSV)
        {
            OperateType = StringBuilderType;
            Handle = new StringBuilder();
        }

        /// <summary>
        /// CSV写入构造器 此方式将使用Stream构造,数据将被直接写入目标Stream
        /// </summary>
        /// <param name="stream"></param>
        public WriteCSVSpreadSheetHandle(Stream stream) : base(SpreadSheetTypeEnum.CSV)
        {
            OperateType = StreamType;
            Handle = AssertKit.AssertNotNull(stream, nameof(stream));
        }

        /// <summary>
        /// 获取对应Bytes
        /// </summary>
        /// <returns>Bytes结果</returns>
        public override byte[] GetBytes()
        {
            if (IsStream())
            {
                var count = GetStreamHandle().Length;
                var result = new char[count];
                using (var tmp = new StreamReader(GetStreamHandle()))
                    return tmp.ReadToEnd().ToUTF8();
            }
            else
                return GetBulderHandle().ToString().ToUTF8();
        }

        /// <summary>
        /// 写入目标流 当目标流与构造函数传入流一致时,不需要调用此方法
        /// </summary>
        /// <param name="stream">目标流</param>
        /// <returns>目标流</returns>
        public override Stream WriteStream(Stream stream)
        {
            AssertKit.AssertNotNull(stream, nameof(stream));
            using (var tmp = new StreamWriter(stream))
            {
                if (IsStream())
                    tmp.Write(GetStreamHandle());
                else
                    tmp.Write(GetBulderHandle().ToString());
            }

            return stream;
        }

        private const int StreamType = 1;
        private const int StringBuilderType = 0;
        private int OperateType { get; set; }
        internal bool IsStream() => OperateType == StreamType;
        internal Stream GetStreamHandle() => (Stream)Handle;
        internal StringBuilder GetBulderHandle() => (StringBuilder)Handle;
        internal bool HasWrited { get; set; }
    }
}