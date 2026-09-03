using SegaCyrus.ToolKits.Model.SpreadSheet.Attributes;

namespace SegaCyrus.ToolKits.Model.Console
{
    internal class CommandArgsHelpModel
    {
        [SpreadSheetColumnDefine("命令前缀串", 1)] public string CommandArgsPrefix { get; set; }

        [SpreadSheetColumnIgnore] public bool IsMulti { get; set; }

        [SpreadSheetColumnDefine("可选项", 3)] public string ParamPool { get; set; }

        [SpreadSheetColumnDefine("描述", 4)] public string Descript { get; set; }

        [SpreadSheetColumnIgnore] public bool IsRequired { get; set; }

        [SpreadSheetColumnIgnore] public bool IsCaseSensitive { get; set; }

        [SpreadSheetColumnDefine("是否必填", 5)] public string IsRequiredString => IsRequired ? "必填" : "非必填";

        [SpreadSheetColumnDefine("命令区分大小写", 5)]
        public string _IsCaseSensitive => IsCaseSensitive ? "区分" : "不区分";

        [SpreadSheetColumnDefine("参数情况", 2)] public string IsMultiString => IsMulti ? "多参数" : "单参数";
    }
}