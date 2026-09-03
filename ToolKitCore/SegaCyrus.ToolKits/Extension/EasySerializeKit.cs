using SegaCyrus.ToolKits.Kit;

namespace SegaCyrus.ToolKits.Extension.SerializeExtension
{
    /// <summary>
    /// 序列化工具包
    /// </summary>
    public static class EasySerializeKit
    {
        #region 序列化

        /// <summary>
        /// 获取格式化后的JSON
        /// </summary>
        /// <typeparam name="T">处理数据类型</typeparam>
        /// <param name="args">待处理数据</param>
        /// <returns>JSON</returns>
        public static string GetFormatJSON<T>(this T args)
        {
            return SerializeKit.GetFormatJSON(args);
        }

        /// <summary>
        /// 获取格式化后的XML
        /// </summary>
        /// <typeparam name="T">处理数据类型</typeparam>
        /// <param name="args">待处理数据</param>
        /// <returns>XML</returns>
        public static string GetFormatXML<T>(this T args)
        {
            return SerializeKit.GetFormatXML(args);
        }

        /// <summary>
        /// XML反序列化对象
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="args">反序列化数据</param>
        /// <returns>对象</returns>
        public static T DesializeXML<T>(this string args)
        {
            return SerializeKit.DeserializeXML<T>(args);
        }

        /// <summary>
        /// JSON反序列化对象
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="args">反序列化数据</param>
        /// <returns>对象</returns>
        public static T DesializeJSON<T>(this string args)
        {
            return SerializeKit.DeserializeJSON<T>(args);
        }

        #endregion
    }
}