using SegaCyrus.ToolKits.Common;
using SegaCyrus.ToolKits.Model.SpreadSheet.Attributes;
using SegaCyrus.ToolKits.Package.SpreadSheetPackage.Base;

namespace SegaCyrus.ToolKits.Package.SpreadSheetPackage
{
    /// <summary>
    /// 电子表格策略工厂
    /// </summary>
    public class SpreadSheetProxy : PolicyPool<SpreadSheetProxy, SpreadSheetRegisterAttribute, SpreadSheetPolicy,
        int, int>
    {
        /// <summary>
        /// 构造Key
        /// </summary>
        /// <param name="attribute"></param>
        /// <param name="instnace"></param>
        /// <returns></returns>
        protected override int BuildPookKey(SpreadSheetRegisterAttribute attribute, SpreadSheetPolicy instnace)
        {
            return attribute.TargetType;
        }

        /// <summary>
        /// 转换Key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        protected override int GetKey(int key)
        {
            return key;
        }
    }
}