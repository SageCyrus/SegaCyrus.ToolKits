using System;
using System.Collections.Generic;
using SegaCyrus.ToolKits.Model.SpreadSheet.Excel;

namespace SegaCyrus.ToolKits.Package.SpreadSheetPackage.Policy.Help
{
    /// <summary>
    /// 默认的枚举下拉实现
    /// </summary>
    internal class BoolExcelDataSource : BaseExcelDataSource
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="type">当前实现属性类型</param>
        public BoolExcelDataSource(Type type) : base(type)
        {
        }

        /// <summary>
        /// 数据池
        /// </summary>
        public override List<string> DataSourcePool()
        {
            return new List<string> { true.ToString(), false.ToString() };
        }
    }
}