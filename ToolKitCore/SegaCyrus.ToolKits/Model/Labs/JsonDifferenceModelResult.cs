using System.Collections.Generic;

namespace SegaCyrus.ToolKits.Model.Labs
{
    /// <summary>
    /// Json对比差异结果
    /// </summary>
    public class JsonDifferenceModelResult
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public JsonDifferenceModelResult()
        {
            DifferenceResult = new List<JsonDifferenceItemModelResult>();
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="args">差异</param>
        public JsonDifferenceModelResult(JsonDifferenceItemModelResult args)
        {
            DifferenceResult = new List<JsonDifferenceItemModelResult>
            {
                args
            };
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="args">差异</param>
        public JsonDifferenceModelResult(List<JsonDifferenceItemModelResult> args)
        {
            DifferenceResult = new List<JsonDifferenceItemModelResult>();
            DifferenceResult.AddRange(args);
        }

        /// <summary>
        /// 差异
        /// </summary>
        public List<JsonDifferenceItemModelResult> DifferenceResult { get; set; }

        /// <summary>
        /// 添加差异
        /// </summary>
        /// <param name="args">差异项</param>
        /// <returns>本实例</returns>
        public JsonDifferenceModelResult Append(JsonDifferenceItemModelResult args)
        {
            DifferenceResult.Add(args);
            return this;
        }
    }
}