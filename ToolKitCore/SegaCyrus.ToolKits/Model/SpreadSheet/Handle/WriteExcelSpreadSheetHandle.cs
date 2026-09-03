using System.Collections.Generic;
using System.IO;
using SegaCyrus.ToolKits.Model.Common;
using SegaCyrus.ToolKits.Model.SpreadSheet.Enums;
using SegaCyrus.ToolKits.Model.SpreadSheet.Excel;
using NPOI.SS.UserModel;

namespace SegaCyrus.ToolKits.Model.SpreadSheet.Append
{
    /// <summary>
    /// excel写入句柄
    /// </summary>
    public class WriteExcelSpreadSheetHandle : WriteSpreadSheetHandle
    {
        /// <summary>
        /// 获取Excel实例
        /// </summary>
        /// <returns></returns>
        public IWorkbook GetWorkBook() => Handle as IWorkbook;

        /// <summary>
        /// 设置Excel实例
        /// </summary>
        /// <param name="handle"></param>
        public void SetWorkBook(IWorkbook handle) => Handle = handle;

        /// <summary>
        /// Excel类型
        /// </summary>
        public ExcelTypeEnum ExcelType { get; internal set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public WriteExcelSpreadSheetHandle() : base(SpreadSheetTypeEnum.EXCEL)
        {
            DataValidatePoolSheetMapVaild =
                new Dictionary<string, Dictionary<string, KVModel<BaseExcelDataSource, IDataValidationConstraint>>>();
        }

        /// <summary>
        /// 写入到目标流
        /// </summary>
        /// <param name="stream">目标流</param>
        /// <returns>目标流</returns>
        public override Stream WriteStream(Stream stream)
        {
            if (GetWorkBook() == null || stream == null)
                return stream;
            GetWorkBook().Write(stream);
            return stream;
        }

        /// <summary>
        /// 数据转Bytes
        /// </summary>
        /// <returns>转换结果</returns>
        public override byte[] GetBytes()
        {
            var handle = GetWorkBook();
            if (handle == null)
                return null;
            using (var stream = new MemoryStream())
            {
                handle.Write(stream);
                return stream.ToArray();
            }
        }

        internal Dictionary<string, Dictionary<string, KVModel<BaseExcelDataSource, IDataValidationConstraint>>>
            DataValidatePoolSheetMapVaild { get; set; }
    }
}