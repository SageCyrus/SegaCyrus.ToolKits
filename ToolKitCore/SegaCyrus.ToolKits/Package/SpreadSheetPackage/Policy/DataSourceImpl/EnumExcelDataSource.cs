using System;
using System.Collections.Generic;
using System.Linq;
using SegaCyrus.ToolKits.Model.SpreadSheet.Excel;

namespace SegaCyrus.ToolKits.Package.SpreadSheetPackage.Policy.Help
{
    /// <summary>
    /// 默认的枚举下拉实现
    /// </summary>
    internal class EnumExcelDataSource : BaseExcelDataSource
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="type">当前实现属性类型</param>
        public EnumExcelDataSource(Type type) : base(type)
        {
        }

        /// <summary>
        /// 数据池
        /// </summary>
        public override List<string> DataSourcePool()
        {
            return Enum.GetNames(PropertyType).ToList();
        }
    }
}