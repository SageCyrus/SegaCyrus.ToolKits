using SegaCyrus.ToolKits.Extension.SpreadsheetExtension.EasyWithConfig.Base;
using SegaCyrus.ToolKits.Model.SpreadSheet.Write;
using System.Data;

namespace SegaCyrus.ToolKits.Extension.SpreadsheetExtension.EasyWithConfig
{
    /// <summary>
    /// DataTabel转Excel
    /// </summary>
    public class EasyDataTableToExcelWithConfig : EasyToExcelConfig<EasyDataTableToExcelWithConfig, DataTable>, IEasyToExcel
    {
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="data"><inheritdoc/></param>
        /// <param name="arg"><inheritdoc/></param>
        public EasyDataTableToExcelWithConfig(DataTable data, WriteExcelConfigSpreadSheetArgs arg) : base(data, arg)
        {
        }
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="data"><inheritdoc/></param>
        /// <param name="arg"><inheritdoc/></param>
        public EasyDataTableToExcelWithConfig(DataTable data, DynamicDataWriteExcelConfigSpreadSheetArgs arg) : base(data, arg)
        {
        }
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns><inheritdoc/></returns>

        public byte[] ToExcel() => Data.ToExcel(this.BuildWriteExcelConfigSpreadSheetArgs());
    }
}