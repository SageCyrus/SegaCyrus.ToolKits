namespace SegaCyrus.ToolKits.Model.SpreadSheet.Convert
{
    /// <summary>
    /// 转换器
    /// </summary>
    public interface ISpreadSheetDataConvert
    {
        /// <summary>
        /// 将数据写入电子表格时候的转换逻辑
        /// </summary>
        /// <param name="value">带写入数据</param>
        /// <returns>转换结果</returns>
        string Write(object value);

        /// <summary>
        /// 将数据从电子表格读取时候的转换逻辑
        /// </summary>
        /// <param name="value">当前电子表格中存储的字符</param>
        /// <returns>转换结果</returns>
        object Read(string value);
    }
}