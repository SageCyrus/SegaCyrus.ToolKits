using System;
using SegaCyrus.ToolKits.Kit;

namespace SegaCyrus.ToolKits.Model.SpreadSheet.Excel.Attributes
{
    /// <summary>
    /// 电子表格列数据源定义
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class ExcelDataSourceAttribute : Attribute
    {
        internal Type DataSourceType { get; set; }

        /// <summary>
        /// 当且仅当对枚举类型使用,对应枚举属性上新增Descript属性即可 否则用枚举Name
        /// </summary>
        public ExcelDataSourceAttribute()
        {
        }

        /// <summary>
        /// 自定义数据源
        /// </summary>
        /// <param name="datasourceBuilder"></param>
        public ExcelDataSourceAttribute(Type datasourceBuilder)
        {
            AssertKit.AssertNotNull(datasourceBuilder, nameof(datasourceBuilder));
            AssertKit.AssertTrue(ReflectKit.IsInhert(datasourceBuilder, typeof(BaseExcelDataSource)),
                "必须继承自SegaCyrus.ToolKits.Model.SpreadSheet.Excel.IExcelDataSource");
            DataSourceType = datasourceBuilder;
        }
    }
}