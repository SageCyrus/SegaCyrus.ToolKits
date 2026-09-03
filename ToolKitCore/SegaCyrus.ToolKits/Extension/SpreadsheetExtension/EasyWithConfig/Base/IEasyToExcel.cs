namespace SegaCyrus.ToolKits.Extension.SpreadsheetExtension.EasyWithConfig.Base
{
    /// <summary>
    /// 转Excel
    /// </summary>
    public interface IEasyToExcel
    {
        /// <summary>
        /// 转Excel
        /// </summary>
        /// <returns>excel二进制数据</returns>
        byte[] ToExcel();
    }
}