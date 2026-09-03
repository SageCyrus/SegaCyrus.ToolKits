using SegaCyrus.ToolKits.Model.SQL.Enums;

namespace SegaCyrus.ToolKits.Model.SQL
{
    /// <summary>
    /// SQL连接配置
    /// </summary>
    public class SQLConnectArgs
    {
        /// <summary>
        /// 目标SQL类型
        /// </summary>
        public SQLTypeEnum SQLType { get; set; }

        /// <summary>
        /// 目标SQL所在主机
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        /// 端口
        /// </summary>
        public uint Port { get; set; }

        /// <summary>
        /// 超时时间
        /// </summary>
        public uint CommandTimeout { get; set; } = 60;

        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// 数据库库名
        /// </summary>
        public string DataSource { get; set; }
    }
}