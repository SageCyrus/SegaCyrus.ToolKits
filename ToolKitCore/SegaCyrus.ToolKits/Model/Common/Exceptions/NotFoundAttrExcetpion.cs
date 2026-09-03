using System;

namespace SegaCyrus.ToolKits.Model.Common.Exceptions
{
    /// <summary>
    /// 没有找到对应的特性
    /// </summary>
    public class NotFoundAttrExcetpion : Exception
    {
        /// <summary>
        /// 没有找到对应的特性
        /// </summary>
        public NotFoundAttrExcetpion()
        {
        }
        /// <summary>
        /// 没有找到对应的特性
        /// </summary>
        /// <param name="message">message</param>
        public NotFoundAttrExcetpion(string message) : base(message)
        {
        }
        /// <summary>
        /// 没有找到对应的特性
        /// </summary>
        /// <param name="type">type</param>
        public NotFoundAttrExcetpion(Type type) : base($"找不到标记的特性类{type?.FullName}")
        {
        }

        /// <summary>
        /// 没有找到对应的特性
        /// </summary>
        /// <param name="message">message</param>
        /// <param name="innerException">innerException</param>
        public NotFoundAttrExcetpion(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
