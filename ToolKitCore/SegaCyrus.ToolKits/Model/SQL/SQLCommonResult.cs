using System.Data;

namespace SegaCyrus.ToolKits.Model.SQL
{
    /// <summary>
    /// SQL执行结果
    /// </summary>
    public class SQLCommonResult
    {
        /// <summary>
        /// 结果映射
        /// </summary>
        public DataTable TableData { get; set; }
    }
}