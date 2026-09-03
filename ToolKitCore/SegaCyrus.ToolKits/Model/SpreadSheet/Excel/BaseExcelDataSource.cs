using System;
using System.Collections.Generic;

namespace SegaCyrus.ToolKits.Model.SpreadSheet.Excel
{
    /// <summary>
    /// Excel下拉选项实现
    /// </summary>
    public abstract class BaseExcelDataSource
    {
        /// <summary>
        /// 当前实现属性类型
        /// </summary>
        protected Type PropertyType { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="type">当前实现属性类型</param>
        public BaseExcelDataSource(Type type)
        {
            PropertyType = type;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public BaseExcelDataSource()
        {
        }

        /// <summary>
        /// 可用下拉项
        /// </summary>
        /// <returns></returns>
        public abstract List<string> DataSourcePool();

        /// <summary>
        /// 提示内容
        /// </summary>
        /// <param name="columnName">当前列名</param>
        /// <returns></returns>
        public virtual string CheckBoxMessage(string columnName) => "请输入或选择下拉列表中的值。";

        /// <summary>
        /// 提示框标题
        /// </summary>
        /// <param name="columnName">当前列名</param>
        /// <returns></returns>
        public virtual string CheckBoxTitle(string columnName) => "错误";
    }
}