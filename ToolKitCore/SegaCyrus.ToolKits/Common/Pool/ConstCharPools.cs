
namespace SegaCyrus.ToolKits.Common.Pool
{
    /// <summary>
    /// 预定义的字符池常量，统一管理，易于扩展。
    /// </summary>
    public class ConstCharPools
    {
        /// <summary>大写字母 A-Z</summary>
        public const string UpperLetters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        /// <summary>小写字母 a-z</summary>
        public const string LowerLetters = "abcdefghijklmnopqrstuvwxyz";
        /// <summary>数字 0-9</summary>
        public const string Digits = "0123456789";
        /// <summary>常见符号</summary>
        public const string Symbols = "`~!@#$%^&*()_+-=[];',./{}:\"<>?";
        /// <summary>字母 + 数字（标准字符池）</summary>
        public const string AlphaNumeric = UpperLetters + LowerLetters + Digits;
        /// <summary>字母 + 数字 + 符号（完整字符池）</summary>
        public const string AllPrintable = AlphaNumeric + Symbols;
        /// <summary>易读字符池（排除 0/O/1/I/l 等易混淆字符）</summary>
        public const string HumanFriendly = "ABCDEFGHJKMNPQRSTUVWXYZabcdefghjkmnpqrstuvwxyz23456789";
        /// <summary>十六进制字符</summary>
        public const string Hex = Digits + "ABCDEF";
    }
}