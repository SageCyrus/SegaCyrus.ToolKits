using SegaCyrus.ToolKits.Extension.SpreadsheetExtension.EasyWithConfig.Base;
using SegaCyrus.ToolKits.Model.SpreadSheet.Write;
using System.Collections.Generic;

namespace SegaCyrus.ToolKits.Extension.SpreadsheetExtension.EasyWithConfig
{
    /// <summary>
    /// 字典集转excel
    /// </summary>
    /// <typeparam name="TKey">Key</typeparam>
    /// <typeparam name="TValue">Value</typeparam>
    public class EasyDicToExcelWithConfig<TKey, TValue> : EasyToExcelConfig<EasyDicToExcelWithConfig<TKey, TValue>, IEnumerable<Dictionary<TKey, TValue>>>, IEasyToExcel
    {
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="data"><inheritdoc/></param>
        /// <param name="arg"><inheritdoc/></param>
        public EasyDicToExcelWithConfig(IEnumerable<Dictionary<TKey, TValue>> data, WriteExcelConfigSpreadSheetArgs arg) : base(data, arg)
        {
        }
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="data"><inheritdoc/></param>
        /// <param name="arg"><inheritdoc/></param>
        public EasyDicToExcelWithConfig(IEnumerable<Dictionary<TKey, TValue>> data, DynamicDataWriteExcelConfigSpreadSheetArgs arg) : base(data, arg)
        {
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns><inheritdoc/></returns>
        public byte[] ToExcel() => Data.ToExcel(this.BuildWriteExcelConfigSpreadSheetArgs());
    }
}