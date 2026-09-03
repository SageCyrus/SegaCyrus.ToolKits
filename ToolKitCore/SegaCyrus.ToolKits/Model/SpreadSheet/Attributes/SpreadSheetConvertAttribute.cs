using System;
using SegaCyrus.ToolKits.Kit;
using SegaCyrus.ToolKits.Model.SpreadSheet.Convert;

namespace SegaCyrus.ToolKits.Model.SpreadSheet.Attributes
{
    /// <summary>
    /// 转换器注册器
    /// </summary>
    public class SpreadSheetConvertAttribute : Attribute
    {
        internal Type ConvertImplType { get; set; }

        /// <summary>
        /// 自定义转换器 必须继承自SegaCyrus.ToolKits.Model.SpreadSheet.Convert.ISpreadSheetDataConvert
        /// </summary>
        /// <param name="convertImpl">转换器</param>
        public SpreadSheetConvertAttribute(Type convertImpl)
        {
            AssertKit.AssertNotNull(convertImpl, nameof(convertImpl));
            AssertKit.AssertTrue(ReflectKit.IsInhert(convertImpl, typeof(ISpreadSheetDataConvert)),
                "必须继承自SegaCyrus.ToolKits.Model.SpreadSheet.Convert.ISpreadSheetDataConvert");
            ConvertImplType = convertImpl;
        }
    }
}