using System;

namespace SegaCyrus.ToolKits.Model.Console.Attributes
{
    /// <summary>
    /// 解析命令行参数属性配置
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class CommandPropertyAttribute : Attribute
    {
        /// <summary>
        /// 短名
        /// </summary>
        public string ShortName { get; set; }

        /// <summary>
        /// 全名
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Descript { get; set; }

        /// <summary>
        /// 是否大小写敏感
        /// </summary>
        public bool IsCaseSensitive { get; set; }

        /// <summary>
        /// 是否必填
        /// </summary>
        public bool IsRequire { get; set; }

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="shortNmae">短名</param>
        public CommandPropertyAttribute(string shortNmae)
        {
            Build(shortNmae, string.Empty, string.Empty, false, false);
        }

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="shortNmae">短名</param>
        /// <param name="fullNmae">全名</param>
        public CommandPropertyAttribute(string shortNmae, string fullNmae)
        {
            Build(shortNmae, fullNmae, string.Empty, false, false);
        }

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="shortNmae">短名</param>
        /// <param name="fullNmae">全名</param>
        /// <param name="descript">描述</param>
        /// <param name="igroneCase">忽略大小写</param>
        /// <param name="isRequire">是否必填</param>
        public CommandPropertyAttribute(string shortNmae, string fullNmae, string descript, bool igroneCase = false,
            bool isRequire = false)
        {
            Build(shortNmae, fullNmae, descript, igroneCase, isRequire);
        }

        private void Build(string shortNmae, string fullName, string descript, bool igroneCase, bool isRequire)
        {
            ShortName = shortNmae;
            FullName = fullName;
            Descript = descript;
            IsCaseSensitive = (igroneCase == false);
            IsRequire = isRequire;
        }
    }
}