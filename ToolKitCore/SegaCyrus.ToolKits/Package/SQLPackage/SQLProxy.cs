//using SegaCyrus.ToolKits.Common;
//using SegaCyrus.ToolKits.Model.SQL.Attributes;
//using SegaCyrus.ToolKits.Model.SQL.Enums;

//namespace SegaCyrus.ToolKits.Package.SQLPackage
//{
//    /// <summary>
//    /// SQL策略工厂
//    /// </summary>
//    public class SQLProxy
//        : PolicyPool<SQLProxy, SQLRegisterAttribute, BaseSQLPolicy, SQLTypeEnum, SQLTypeEnum>
//    {
//        /// <summary>
//        /// 通过实例和属性输出此策略的唯一标识
//        /// </summary>
//        /// <param name="attribute">此策略实例被标记的属性</param>
//        /// <param name="instnace">策略实例</param>
//        /// <returns>策略key</returns>
//        protected override SQLTypeEnum BuildPookKey(SQLRegisterAttribute attribute, BaseSQLPolicy instnace)
//            => attribute.TargetType;

//        /// <summary>
//        /// key转换方法
//        /// </summary>
//        /// <param name="key">key</param>
//        /// <returns>内部key</returns>
//        protected override SQLTypeEnum GetKey(SQLTypeEnum key) => key;
//    }
//}