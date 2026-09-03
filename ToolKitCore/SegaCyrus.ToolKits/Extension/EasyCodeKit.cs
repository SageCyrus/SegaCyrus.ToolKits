using System.Runtime.CompilerServices;
using SegaCyrus.ToolKits.Kit;

namespace SegaCyrus.ToolKits.Extension.CodeExtension
{
    /// <summary>
    /// CodeKit扩展包
    /// </summary>
    public static class EasyCodeKit
    {
        #region 信息编码转换

        /// <summary>
        /// 剔除转移符
        /// </summary>
        /// <param name="content">待处理数据</param>
        /// <returns>处理结果</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string CharsToString(this char[] content)
        {
            return CodeKit.CharArrToString(content);
        }

        /// <summary>
        /// 剔除转移符
        /// </summary>
        /// <param name="content">待处理数据</param>
        /// <returns>处理结果</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string RemoveUnescape(this string content)
        {
            return CodeKit.RemoveUnescape(content);
        }

        /// <summary>
        /// 字符串转UTF8字节串
        /// </summary>
        /// <param name="args">待处理数据</param>
        /// <returns>处理结果</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte[] ToUTF8(this string args)
        {
            return CodeKit.StringToUTF8(args);
        }

        /// <summary>
        /// UTF8转字符串
        /// </summary>
        /// <param name="args">待处理数据</param>
        /// <returns>处理结果</returns>
        public static string UTF8ToString(this byte[] args)
        {
            return CodeKit.UTF8ToString(args);
        }

        /// <summary>
        /// 字符串进行base64加码
        /// </summary>
        /// <param name="args">待处理数据</param>
        /// <returns>处理结果</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ToBase64(this string args)
        {
            return CodeKit.StringToBase64(args);
        }

        /// <summary>
        /// 字节串进行base64加码
        /// </summary>
        /// <param name="args">待处理数据</param>
        /// <returns>处理结果</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ToBase64(this byte[] args)
        {
            return CodeKit.BytesToBase64(args);
        }

        /// <summary>
        /// base64解码为字符串
        /// </summary>
        /// <param name="base64">待处理数据</param>
        /// <returns>处理结果</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string Bas64ToString(this string base64)
        {
            return CodeKit.Base64ToString(base64);
        }

        /// <summary>
        /// base64解码为字节串
        /// </summary>
        /// <param base64="args">待处理数据</param>
        /// <returns>处理结果</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte[] Base64ToBytes(this string base64)
        {
            return CodeKit.Base64ToBytes(base64);
        }

        #endregion

        /// <summary>
        /// MD5散列值
        /// </summary>
        /// <param name="input">待处理数据</param>
        /// <returns>处理结果</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string SHA256(this string input)
        {
            return CodeKit.SHA256Hash(input);
        }

        /// <summary>
        /// MD5散列值
        /// </summary>
        /// <param name="input">待处理数据</param>
        /// <returns>处理结果</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string SHA256(this byte[] input)
        {
            return CodeKit.SHA256Hash(input);
        }

        /// <summary>
        /// MD5散列值
        /// </summary>
        /// <param name="input">待处理数据</param>
        /// <returns>处理结果</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string MD5(this string input)
        {
            return CodeKit.MD5Hash(input);
        }

        /// <summary>
        /// MD5散列值
        /// </summary>
        /// <param name="input">待处理数据</param>
        /// <returns>处理结果</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string MD5(this byte[] input)
        {
            return CodeKit.MD5Hash(input);
        }

        /// <summary>
        /// AES加密
        /// </summary>
        /// <param name="input">待处理数据</param>
        /// <returns>处理结果</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte[] AESEncrypt(this byte[] input)
        {
            return CodeKit.AESEncrypt(input);
        }

        /// <summary>
        /// AES解密
        /// </summary>
        /// <param name="input">待处理数据</param>
        /// <returns>处理结果</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte[] AESDecrypt(this byte[] input)
        {
            return CodeKit.AESDecrypt(input);
        }
    }
}