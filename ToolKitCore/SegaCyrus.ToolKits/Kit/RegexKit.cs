using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace SegaCyrus.ToolKits.Kit
{
    /// <summary>
    /// 常用正则包
    /// </summary>
    public class RegexKit
    {
        /// <summary>
        /// 检查给定字符串是否符合变量命名规范的正则
        /// </summary>
        public static Regex VarialbeNamedRegeix { get; } = new Regex("^[a-zA-Z_][a-zA-Z_0-9]*$", RegexOptions.Compiled);

        /// <summary>
        /// 检查给定字符串是否符合变量命名规范
        /// </summary>
        /// <param name="variableName">待检查数据</param>
        /// <returns>检查结果</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool CheckVariableNameSpecification(string variableName) =>
            VarialbeNamedRegeix.IsMatch(variableName);
    }
}