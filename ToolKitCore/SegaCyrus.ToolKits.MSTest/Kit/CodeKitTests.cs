using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SegaCyrus.ToolKits.Kit;

namespace SegaCyrus.ToolKits.MSTest.Kit
{
    /// <summary>
    /// <see cref="CodeKit"/> 单元测试
    /// </summary>
    [TestClass]
    public class CodeKitTests
    {
        private const string SampleText = "你好，SegaCyrus.ToolKits！ Hello 123 ~!@#";

        #region CharArrToString

        [TestMethod]
        public void CharArrToString_Chars_ReturnsString()
        {
            char[] chars = { 'a', 'b', 'c', '中' };
            Assert.AreEqual("abc中", CodeKit.CharArrToString(chars));
        }

        [TestMethod]
        public void CharArrToString_Empty_ReturnsEmpty()
        {
            Assert.AreEqual(string.Empty, CodeKit.CharArrToString(new char[0]));
        }

        [TestMethod]
        public void CharArrToString_Null_ReturnsEmpty()
        {
            Assert.AreEqual(string.Empty, CodeKit.CharArrToString(null));
        }

        #endregion

        #region RemoveUnescape

        [TestMethod]
        [DataRow(@"a\tb", "a\tb")]
        [DataRow(@"a\nb", "a\nb")]
        [DataRow(@"a\.b\*c", "a.b*c")]
        public void RemoveUnescape_Value_ReturnsUnescaped(string input, string expected)
        {
            Assert.AreEqual(expected, CodeKit.RemoveUnescape(input));
        }

        [TestMethod]
        public void RemoveUnescape_Empty_Throws()
        {
            Assert.ThrowsExactly<ArgumentException>(() => CodeKit.RemoveUnescape(string.Empty));
        }

        #endregion

        #region UTF8

        [TestMethod]
        public void StringToUTF8_UTF8ToString_RoundTrip()
        {
            var bytes = CodeKit.StringToUTF8(SampleText);
            Assert.IsNotEmpty(bytes);
            Assert.AreEqual(SampleText, CodeKit.UTF8ToString(bytes));
        }

        [TestMethod]
        public void StringToUTF8_Empty_Throws()
        {
            Assert.ThrowsExactly<ArgumentException>(() => CodeKit.StringToUTF8(string.Empty));
        }

        [TestMethod]
        public void UTF8ToString_Null_Throws()
        {
            Assert.ThrowsExactly<ArgumentNullException>(() => CodeKit.UTF8ToString(null));
        }

        #endregion

        #region Base64

        [TestMethod]
        public void StringToBase64_Base64ToString_RoundTrip()
        {
            var base64 = CodeKit.StringToBase64(SampleText);
            Assert.AreEqual(Convert.ToBase64String(Encoding.UTF8.GetBytes(SampleText)), base64);
            Assert.AreEqual(SampleText, CodeKit.Base64ToString(base64));
        }

        [TestMethod]
        public void BytesToBase64_Base64ToBytes_RoundTrip()
        {
            byte[] data = { 0x00, 0x01, 0x02, 0xFE, 0xFF };
            var base64 = CodeKit.BytesToBase64(data);
            CollectionAssert.AreEqual(data, CodeKit.Base64ToBytes(base64));
        }

        [TestMethod]
        public void Base64ToString_InvalidBase64_Throws()
        {
            Assert.ThrowsExactly<FormatException>(() => CodeKit.Base64ToString("@@@not-base64@@@"));
        }

        [TestMethod]
        public void StringToBase64_Empty_Throws()
        {
            Assert.ThrowsExactly<ArgumentException>(() => CodeKit.StringToBase64(string.Empty));
        }

        [TestMethod]
        public void Base64ToBytes_Empty_Throws()
        {
            Assert.ThrowsExactly<ArgumentException>(() => CodeKit.Base64ToBytes(string.Empty));
        }

        #endregion

        #region GZip

        [TestMethod]
        public void GZipCompress_String_DecompressToString_RoundTrip()
        {
            var compressed = CodeKit.GZipCompress(SampleText);
            Assert.IsNotEmpty(compressed);
            Assert.AreEqual(SampleText, CodeKit.DecompressToString(compressed));
        }

        [TestMethod]
        public void GZipCompress_Bytes_Decompress_RoundTrip()
        {
            var source = Encoding.UTF8.GetBytes(SampleText);
            var compressed = CodeKit.GZipCompress(source);
            var decompressed = CodeKit.Decompress(compressed);
            CollectionAssert.AreEqual(source, decompressed);
        }

        [TestMethod]
        public void GZipCompress_EmptyString_ReturnsEmptyBytes()
        {
            var result = CodeKit.GZipCompress(string.Empty);
            Assert.IsNotNull(result);
            Assert.IsEmpty(result);
        }

        [TestMethod]
        public void GZipCompress_NullBytes_ReturnsNull()
        {
            Assert.IsNull(CodeKit.GZipCompress((byte[])null));
        }

        [TestMethod]
        public void Decompress_Null_ReturnsNull()
        {
            Assert.IsNull(CodeKit.Decompress(null));
        }

        [TestMethod]
        public void DecompressToString_NotGZip_Throws()
        {
            Assert.ThrowsExactly<System.IO.InvalidDataException>(
                () => CodeKit.DecompressToString(new byte[] { 1, 2, 3, 4, 5 }));
        }

        #endregion

        #region MD5

        [TestMethod]
        [DataRow("abc", "900150983cd24fb0d6963f7d28e17f72")]
        [DataRow("hello", "5d41402abc4b2a76b9719d911017c592")]
        public void MD5Hash_String_MatchesKnownValue(string input, string expected)
        {
            Assert.AreEqual(expected, CodeKit.MD5Hash(input));
        }

        [TestMethod]
        public void MD5Hash_EmptyString_Throws()
        {
            Assert.ThrowsExactly<ArgumentException>(() => CodeKit.MD5Hash(string.Empty));
        }

        [TestMethod]
        public void MD5Hash_Utf8Content_IsStableAnd32Hex()
        {
            var hash = CodeKit.MD5Hash(SampleText);
            Assert.AreEqual(32, hash.Length);
            Assert.AreEqual(hash, CodeKit.MD5Hash(SampleText));
        }

        #endregion

        #region SHA256

        [TestMethod]
        [DataRow("abc", "ba7816bf8f01cfea414140de5dae2223b00361a396177a9cb410ff61f20015ad")]
        public void SHA256Hash_String_MatchesKnownValue(string input, string expected)
        {
            Assert.AreEqual(expected, CodeKit.SHA256Hash(input));
        }

        [TestMethod]
        public void SHA256Hash_EmptyString_Throws()
        {
            Assert.ThrowsExactly<ArgumentException>(() => CodeKit.SHA256Hash(string.Empty));
        }

        [TestMethod]
        public void SHA256Hash_Bytes_MatchesStringVersion()
        {
            var expected = CodeKit.SHA256Hash(SampleText);
            var actual = CodeKit.SHA256Hash(Encoding.UTF8.GetBytes(SampleText));
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void SHA256Hash_NullString_Throws()
        {
            Assert.ThrowsExactly<ArgumentNullException>(() => CodeKit.SHA256Hash((string)null));
        }

        #endregion

        #region AES

        [TestMethod]
        public void AESEncrypt_AESDecrypt_RoundTrip_WithDefaultKey()
        {
            var source = Encoding.UTF8.GetBytes(SampleText);
            var encrypted = CodeKit.AESEncrypt(source);
            Assert.IsFalse(source.SequenceEqual(encrypted));
            var decrypted = CodeKit.AESDecrypt(encrypted);
            CollectionAssert.AreEqual(source, decrypted);
        }

        [TestMethod]
        public void AESEncrypt_AESDecrypt_RoundTrip_WithCustomKeyAndIv()
        {
            const string key = "0123456789abcdef";
            const string iv = "abcdef0123456789";
            var source = Encoding.UTF8.GetBytes(SampleText);
            var encrypted = CodeKit.AESEncrypt(source, key, iv);
            var decrypted = CodeKit.AESDecrypt(encrypted, key, iv);
            CollectionAssert.AreEqual(source, decrypted);
        }

        [TestMethod]
        public void AESEncrypt_WithECBMode_RoundTrip()
        {
            var source = Encoding.UTF8.GetBytes("ECB mode round trip test");
            var encrypted = CodeKit.AESEncrypt(source, "secret-key-16b", "",
                PaddingMode.PKCS7, CipherMode.ECB);
            var decrypted = CodeKit.AESDecrypt(encrypted, "secret-key-16b", "",
                PaddingMode.PKCS7, CipherMode.ECB);
            CollectionAssert.AreEqual(source, decrypted);
        }

        [TestMethod]
        public void AESEncrypt_SameKeyIv_SameCiphertext()
        {
            var source = Encoding.UTF8.GetBytes("deterministic");
            var a = CodeKit.AESEncrypt(source, "0123456789abcdef", "abcdef0123456789");
            var b = CodeKit.AESEncrypt(source, "0123456789abcdef", "abcdef0123456789");
            CollectionAssert.AreEqual(a, b);
        }

        [TestMethod]
        public void AESDecrypt_InvalidData_Throws()
        {
            Assert.ThrowsExactly<CryptographicException>(
                () => CodeKit.AESDecrypt(new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 }));
        }

        #endregion
    }
}
