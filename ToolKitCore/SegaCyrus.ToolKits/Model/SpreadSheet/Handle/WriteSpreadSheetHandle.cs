using System.IO;
using SegaCyrus.ToolKits.Model.SpreadSheet.Enums;

namespace SegaCyrus.ToolKits.Model.SpreadSheet.Append
{
    /// <summary>
    /// 电子表格句柄类
    /// </summary>
    public abstract class WriteSpreadSheetHandle
    {
        /// <summary>
        /// 操作句柄
        /// </summary>
        internal object Handle { get; set; }

        /// <summary>
        /// 操作类型
        /// </summary>
        internal int TypeEnum { get; private set; }

        /// <summary>
        /// 构造函数 根据Type派发
        /// </summary>
        /// <param name="type">类型</param>
        protected WriteSpreadSheetHandle(int type)
        {
            TypeEnum = type;
        }

        /// <summary>
        /// 构造函数 根据Type派发
        /// </summary>
        /// <param name="type">类型</param>
        protected WriteSpreadSheetHandle(SpreadSheetTypeEnum type)
        {
            TypeEnum = (int)type;
        }

        /// <summary>
        /// 数据转Bytes
        /// </summary>
        /// <returns>转换结果</returns>
        public abstract byte[] GetBytes();

        /// <summary>
        /// 写入到目标流
        /// </summary>
        /// <param name="stream">目标流</param>
        /// <returns>目标流</returns>
        public abstract Stream WriteStream(Stream stream);
    }
}