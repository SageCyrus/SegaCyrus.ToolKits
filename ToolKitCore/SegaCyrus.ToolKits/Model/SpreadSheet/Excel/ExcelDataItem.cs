using System.Collections.Generic;

namespace SegaCyrus.ToolKits.Model.SpreadSheet.Excel
{
    /// <summary>
    /// Excel数据行简单封装
    /// </summary>
    public class ExcelDataItem
    {
        private Dictionary<string, int> ColumnName { get; set; }
        private List<string> RowData { get; set; }

        internal ExcelDataItem(Dictionary<string, int> columnName, List<string> rowData)
        {
            ColumnName = columnName;
            RowData = rowData;
        }

        internal ExcelDataItem UpdateRowData(List<string> rowData)
        {
            RowData = rowData;
            return this;
        }

        /// <summary>
        /// 获取指定列名数据
        /// </summary>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public string this[string columnName]
        {
            get => RowData[ColumnName[columnName]];
            set => RowData[ColumnName[columnName]] = value;
        }
    }
}