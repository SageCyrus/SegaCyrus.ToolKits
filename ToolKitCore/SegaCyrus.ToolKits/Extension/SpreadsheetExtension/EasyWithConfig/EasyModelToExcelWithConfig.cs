using SegaCyrus.ToolKits.Extension.SpreadsheetExtension.EasyWithConfig.Base;
using SegaCyrus.ToolKits.Model.SpreadSheet.Write;
using System.Collections.Generic;

namespace SegaCyrus.ToolKits.Extension.SpreadsheetExtension.EasyWithConfig
{
    /// <summary>
    /// 强模型转Excel
    /// </summary>
    /// <typeparam name="TData">待转模型</typeparam>
    public class EasyModelToExcelWithConfig<TData> : EasyToExcelConfig<EasyModelToExcelWithConfig<TData>, IEnumerable<TData>>, IEasyToExcel
#if NET_8
            where TData notnul;
#else
            where TData : class
#endif
    {
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="data"><inheritdoc/></param>
        /// <param name="arg"><inheritdoc/></param>
        public EasyModelToExcelWithConfig(IEnumerable<TData> data, WriteExcelConfigSpreadSheetArgs arg) : base(data, arg)
        {
        }
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="data"><inheritdoc/></param>
        /// <param name="arg"><inheritdoc/></param>
        public EasyModelToExcelWithConfig(IEnumerable<TData> data, DynamicDataWriteExcelConfigSpreadSheetArgs arg) : base(data, arg)
        {
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns><inheritdoc/></returns>
        public byte[] ToExcel()
            => Data.ToExcel(this.BuildWriteExcelConfigSpreadSheetArgs());
    }
}