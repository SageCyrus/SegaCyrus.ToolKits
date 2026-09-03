using SegaCyrus.ToolKits.Model.Labs.Enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace SegaCyrus.ToolKits.Model.Labs
{
    /// <summary>
    /// Json差异项
    /// </summary>
    public class JsonDifferenceItemModelResult
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="differenceType">数据类型</param>
        /// <param name="sourceValue">源</param>
        /// <param name="targetValue">目标</param>
        /// <param name="moreInfor">更多信息</param>
        public JsonDifferenceItemModelResult(string path, DifferenceTypeEnum differenceType, string sourceValue = null,
            string targetValue = null, string moreInfor = null)
        {
            Path = path;
            DifferenceType = differenceType;
            SourceValue = sourceValue;
            TargetValue = targetValue;
            MoreInformation = moreInfor;
        }

        /// <summary>
        /// 差异类型
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public DifferenceTypeEnum DifferenceType { get; set; }

        /// <summary>
        /// 路径
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// 源
        /// </summary>
        public string SourceValue { get; set; }

        /// <summary>
        /// 详细信息
        /// </summary>
        public string MoreInformation { get; set; }

        /// <summary>
        /// 目标
        /// </summary>
        public string TargetValue { get; set; }
    }
}