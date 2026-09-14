using SegaCyrus.ToolKits.Extension.CodeExtension;
using SegaCyrus.ToolKits.Extension.CommonExtension;
using NPOI.SS.Formula.Functions;
using System;
using System.IO;
using System.IO.Compression;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace SegaCyrus.ToolKits.Kit
{
    /// <summary>
    /// 编码工具包
    /// </summary>
    public class CodeKit
    {
        private const string AESKey = "2C18B85E167E4023BD3802CD333B57D9";
        private static UTF8Encoding UTF8Encoding= new UTF8Encoding(false);

        /// <summary>
        /// char[]转字符串
        /// </summary>
        /// <param name="chars">转换目标</param>
        /// <returns>结果</returns>
        public static string CharArrToString(char[] chars)
        {
            if (chars.IsEmpty())
                return string.Empty;
            var sb = new StringBuilder(chars.Length);
            sb.Append(chars);
            return sb.ToString();
        }

        /// <summary>
        /// 字符串转UTF8
        /// </summary>
        /// <param name="str">带转换串</param>
        /// <returns>UTF8编码</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string RemoveUnescape(string str)
        {
            AssertKit.AssertNotEmpty(str, nameof(str));
            return Regex.Unescape(str);
        }

        #region UTF8

        /// <summary>
        /// 字符串转UTF8
        /// </summary>
        /// <param name="str">带转换串</param>
        /// <returns>UTF8编码</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte[] StringToUTF8(string str)
        {
            AssertKit.AssertNotEmpty(str, nameof(str));
            return UTF8Encoding.GetBytes(str);
        }

        /// <summary>
        /// UTF8转字符串
        /// </summary>
        /// <param name="data">UTF8内容</param>
        /// <returns>字符串</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string UTF8ToString(byte[] data)
        {
            AssertKit.AssertNotNull(data, nameof(data));
            return UTF8Encoding.GetString(data);
        }

        #endregion

        #region Base64

        /// <summary>
        /// 字符串转Base64
        /// </summary>
        /// <param name="str">待转换内容</param>
        /// <returns>Base64</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string StringToBase64(string str)
        {
            AssertKit.AssertNotEmpty(str, nameof(str));
            return Convert.ToBase64String(StringToUTF8(str));
        }

        /// <summary>
        /// Base64转字符串
        /// </summary>
        /// <param name="base64">待转换内容</param>
        /// <returns>字符串</returns>
        public static string Base64ToString(string base64)
        {
            AssertKit.AssertNotEmpty(base64, nameof(base64));
            return UTF8ToString(Convert.FromBase64String(base64));
        }

        /// <summary>
        /// 二进制内容转Base64
        /// </summary>
        /// <param name="data">待转换内容</param>
        /// <returns>Base64</returns>
        public static string BytesToBase64(byte[] data)
        {
            AssertKit.AssertNotNull(data, nameof(data));
            return Convert.ToBase64String(data);
        }

        /// <summary>
        /// base64转字符串
        /// </summary>
        /// <param name="base64">base64</param>
        /// <returns>字符串</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte[] Base64ToBytes(string base64)
        {
            AssertKit.AssertNotEmpty(base64, nameof(base64));
            return Convert.FromBase64String(base64);
        }

        #endregion

        #region GZip

        /// <summary>
        /// 压缩指定字符串
        /// </summary>
        /// <param name="str">待压缩字符串</param>
        /// <returns>返回压缩后的字节数组</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte[] GZipCompress(string str)
        {
            if (string.IsNullOrEmpty(str))
                return new byte[0];
            return GZipCompress(str.ToUTF8());
        }

        /// <summary>
        /// 压缩指定字节数组
        /// </summary>
        /// <param name="bytes">待压缩字节数组</param>
        /// <returns>返回压缩后的字节数组</returns>
        public static byte[] GZipCompress(byte[] bytes)
        {
            if (bytes == null || bytes.Length <= 0) return bytes;

            using (var compressedStream = new MemoryStream())
            {
                using (var compressionStream = new GZipStream(compressedStream, CompressionMode.Compress))
                {
                    compressionStream.Write(bytes, 0, bytes.Length);
                }

                return compressedStream.ToArray();
            }
        }

        /// <summary>
        /// 从指定字节数组解压出字符串
        /// </summary>
        /// <param name="bytes">待解压的字节数组</param>
        /// <returns>返回解压后的字符串</returns>
        public static string DecompressToString(byte[] bytes)
        {
            var result = Decompress(bytes);
            if (result == null || result.Length <= 0)
                return string.Empty;
            return result.UTF8ToString();
        }

        /// <summary>
        /// 从指定字节数组解压出字节数组
        /// </summary>
        /// <param name="bytes">待解压的字节数组</param>
        /// <returns>返回解压后的字节数组</returns>
        public static byte[] Decompress(byte[] bytes)
        {
            if (bytes == null || bytes.Length <= 0) return bytes;

            using (var originalStream = new MemoryStream(bytes))
            {
                using (var decompressedStream = new MemoryStream())
                {
                    using (var decompressionStream = new GZipStream(originalStream, CompressionMode.Decompress))
                    {
                        decompressionStream.CopyTo(decompressedStream);
                    }

                    return decompressedStream.ToArray();
                }
            }
        }

        #endregion

        #region MD5

        /// <summary>
        /// 生成MD5
        /// </summary>
        /// <param name="data">待处理数据</param>
        /// <returns>MD5结果</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string MD5Hash(string data)
        {
            return MD5Hash(data.ToUTF8());
        }

        /// <summary>
        /// 生成MD5
        /// </summary>
        /// <param name="data">待处理数据</param>
        /// <returns>MD5结果</returns>
        public static string MD5Hash(byte[] data)
        {
#if NET6_0_OR_GREATER
            var hashBytes = MD5.HashData(data);
            return Convert.ToHexString(hashBytes).ToLowerInvariant();
#else 
            using (var md5 = MD5.Create())
            {
                var hashBytes = md5.ComputeHash(data);
                return ToHexString(hashBytes, upper: false);
            }
#endif
        }

        #endregion

        #region SHA

        /// <summary>
        /// 计算SHA256摘要（UTF-8无BOM编码）
        /// </summary>
        /// <param name="data">原始字符串</param>
        /// <returns>小写SHA256十六进制摘要</returns>
        public static string SHA256Hash(string data)
        {
            AssertKit.AssertNotNull(data, nameof(data));

            // 使用无BOM UTF8，标准哈希通用规范
            byte[] bytes = StringToUTF8(data);
            return SHA256Hash(bytes);
        }

        /// <summary>
        /// 计算SHA256摘要
        /// </summary>
        /// <param name="data">原始字节</param>
        /// <returns>小写SHA256十六进制摘要</returns>
        public static string SHA256Hash(byte[] data)
        {
            AssertKit.AssertNotNull(data, nameof(data));
#if NET6_0_OR_GREATER
            byte[] hashValue = SHA256.HashData(data);
            // Convert.ToHexString 默认大写，统一转小写，保证跨框架一致
            return Convert.ToHexString(hashValue).ToLowerInvariant();
#else
            using (var sha256 = SHA256.Create())
            {
                byte[] hashValue = sha256.ComputeHash(data);
                return ToHexString(hashValue, upper: false);
            }
#endif
        }

#if !NET6_0_OR_GREATER
        /// <summary>
        /// 字节数组转十六进制字符串
        /// </summary>
        private static string ToHexString(byte[] bytes, bool upper = false)
        {
            AssertKit.AssertNotNull(bytes, nameof(bytes));
            var sb = new StringBuilder(bytes.Length * 2);
            string fmt = upper ? "X2" : "x2";
            foreach (byte b in bytes)
                sb.Append(b.ToString(fmt));
            return sb.ToString();
        }
#endif

        #endregion

        #region ASE

        /// <summary>
        /// AES 加密。
        /// 密钥按 UTF-8 取字节后规范化为合法长度（16/24/32 字节 → AES-128/192/256）。
        /// 当 <paramref name="iv"/> 为空且为 CBC 模式时，自动生成密码学安全的随机 IV 并前置拼接到返回的密文头部；
        /// 传入非空 <paramref name="iv"/>，或为非 CBC 模式时，按原格式处理（不前置，需与加密端一致）。
        /// 注意：iv 为空时的输出格式与旧版本（固定零 IV、无前置）不兼容，旧密文需重新加密。
        /// </summary>
        /// <param name="source">明文</param>
        /// <param name="key">密钥（支持任意长度，自动规范化为 16/24/32 字节）</param>
        /// <param name="iv">初始向量；为空且 CBC 时随机生成并前置</param>
        /// <param name="padding">填充模式</param>
        /// <param name="mode">加密模式</param>
        /// <returns>密文（iv 为空且 CBC 时含前置 IV）</returns>
        public static byte[] AESEncrypt(byte[] source, string key = AESKey, string iv = "",
            PaddingMode padding = PaddingMode.PKCS7, CipherMode mode = CipherMode.CBC)
        {
            bool prependIv = string.IsNullOrEmpty(iv) && mode == CipherMode.CBC;
            using (var aes = Aes.Create())
            {
                aes.BlockSize = 128;
                aes.Padding = padding;
                aes.Mode = mode;
                aes.Key = NormalizeAesKey(key);

                byte[] ivBytes = prependIv ? RandomAesIv(aes.BlockSize / 8) : NormalizeAesIv(iv, aes.BlockSize / 8);
                aes.IV = ivBytes;

                using (var encryptor = aes.CreateEncryptor())
                using (var ms = new MemoryStream())
                {
                    if (prependIv)
                        ms.Write(ivBytes, 0, ivBytes.Length);
                    using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                    {
                        cs.Write(source, 0, source.Length);
                        cs.FlushFinalBlock();
                    }
                    return ms.ToArray();
                }
            }
        }

        /// <summary>
        /// AES 解密。
        /// 密钥处理同 <see cref="AESEncrypt"/>。
        /// 当 <paramref name="iv"/> 为空且为 CBC 模式时，从密文头部读取前置的随机 IV（与新的 AESEncrypt 配对）；
        /// 传入非空 <paramref name="iv"/>，或为非 CBC 模式时，按原格式从头部开始解密。
        /// </summary>
        /// <param name="source">密文</param>
        /// <param name="key">密钥</param>
        /// <param name="iv">初始向量；为空且 CBC 时从密文头部读取前置 IV</param>
        /// <param name="padding">填充模式</param>
        /// <param name="mode">加密模式</param>
        /// <returns>明文</returns>
        public static byte[] AESDecrypt(byte[] source, string key = AESKey, string iv = "",
            PaddingMode padding = PaddingMode.PKCS7, CipherMode mode = CipherMode.CBC)
        {
            using (var aes = Aes.Create())
            {
                aes.BlockSize = 128;
                aes.Padding = padding;
                aes.Mode = mode;
                aes.Key = NormalizeAesKey(key);

                byte[] ivBytes;
                int dataOffset;
                if (string.IsNullOrEmpty(iv) && mode == CipherMode.CBC)
                {
                    int ivLen = aes.BlockSize / 8;
                    if (source == null || source.Length < ivLen)
                        throw new ArgumentException("密文长度不足，无法读取前置 IV。", nameof(source));
                    ivBytes = new byte[ivLen];
                    Array.Copy(source, 0, ivBytes, 0, ivLen);
                    dataOffset = ivLen;
                }
                else
                {
                    ivBytes = NormalizeAesIv(iv, aes.BlockSize / 8);
                    dataOffset = 0;
                }
                aes.IV = ivBytes;

                using (var decryptor = aes.CreateDecryptor())
                using (var ms = new MemoryStream(source, dataOffset, source.Length - dataOffset))
                using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                using (var outMs = new MemoryStream())
                {
                    cs.CopyTo(outMs);
                    return outMs.ToArray();
                }
            }
        }

        /// <summary>
        /// 将密钥规范化为合法 AES 密钥长度（16/24/32 字节）。
        /// UTF-8 字节长度 ≥32 取 32，≥24 取 24，否则取 16（不足补零）。
        /// </summary>
        private static byte[] NormalizeAesKey(string key)
        {
            var keyBytes = Encoding.UTF8.GetBytes(key ?? string.Empty);
            int size = keyBytes.Length >= 32 ? 32 : (keyBytes.Length >= 24 ? 24 : 16);
            var result = new byte[size];
            Array.Copy(keyBytes, result, Math.Min(keyBytes.Length, size));
            return result;
        }

        /// <summary>
        /// 生成密码学安全的随机 IV。
        /// </summary>
        private static byte[] RandomAesIv(int length)
        {
            var iv = new byte[length];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(iv);
            }
            return iv;
        }

        /// <summary>
        /// 将 IV 字符串规范化为指定长度（不足补零，超长截断）。
        /// </summary>
        private static byte[] NormalizeAesIv(string iv, int length)
        {
            var ivBytes = Encoding.UTF8.GetBytes(iv ?? string.Empty);
            var result = new byte[length];
            Array.Copy(ivBytes, result, Math.Min(ivBytes.Length, length));
            return result;
        }

        #endregion
    }
}