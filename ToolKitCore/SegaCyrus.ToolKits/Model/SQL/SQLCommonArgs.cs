namespace SegaCyrus.ToolKits.Model.SQL
{
    /// <summary>
    /// SQL执行参数
    /// </summary>
    public class SQLCommonArgs : SQLConnectArgs
    {
        /// <summary>
        /// 待执行SQL内容
        /// </summary>
        public string ExcuteSQL { get; set; }
    }
}