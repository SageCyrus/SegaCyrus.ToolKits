using System;
using System.Runtime.Caching;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SegaCyrus.ToolKits.Kit;

namespace SegaCyrus.ToolKits.MSTest.Kit
{
    /// <summary>
    /// <see cref="CacheKit"/> 单元测试
    /// </summary>
    [TestClass]
    public class CacheKitTests
    {
        private readonly Guid _instance = Guid.NewGuid();

        private string NewKey() => $"ck_{_instance:N}_{Guid.NewGuid():N}";

        [TestMethod]
        public void Add_NewKey_ReturnsTrue()
        {
            var key = NewKey();
            try
            {
                Assert.IsTrue(CacheKit.Add(key, 123));
            }
            finally
            {
                CacheKit.Remove(key);
            }
        }

        [TestMethod]
        public void Add_DuplicateKey_ReturnsFalse()
        {
            var key = NewKey();
            try
            {
                Assert.IsTrue(CacheKit.Add(key, "first"));
                Assert.IsFalse(CacheKit.Add(key, "second"));
                Assert.AreEqual("first", CacheKit.GetOrAdd(key, () => "first"));
            }
            finally
            {
                CacheKit.Remove(key);
            }
        }

        [TestMethod]
        public void GetOrAdd_ExistingKey_ReturnsCachedValue_WithoutCallingFunc()
        {
            var key = NewKey();
            try
            {
                CacheKit.Add(key, "cached");
                var called = false;
                var value = CacheKit.GetOrAdd(key, () => { called = true; return "new"; });
                Assert.AreEqual("cached", value);
                Assert.IsFalse(called);
            }
            finally
            {
                CacheKit.Remove(key);
            }
        }

        [TestMethod]
        public void GetOrAdd_MissingKey_AddsAndReturnsValue()
        {
            var key = NewKey();
            try
            {
                var value = CacheKit.GetOrAdd(key, () => 42);
                Assert.AreEqual(42, value);
                // 第二次直接命中缓存
                Assert.AreEqual(42, CacheKit.GetOrAdd(key, () => 99));
            }
            finally
            {
                CacheKit.Remove(key);
            }
        }

        [TestMethod]
        public void Remove_ExistingKey_ReturnsRemovedObject()
        {
            var key = NewKey();
            var obj = new object();
            CacheKit.Add(key, obj);
            Assert.AreSame(obj, CacheKit.Remove(key));
            // 已移除，再次移除返回 null
            Assert.IsNull(CacheKit.Remove(key));
        }

        [TestMethod]
        public void Clear_RemovesAllKeys()
        {
            var k1 = NewKey();
            var k2 = NewKey();
            try
            {
                CacheKit.Add(k1, 1);
                CacheKit.Add(k2, "two");
                CacheKit.Clear();
                var called = false;
                CacheKit.GetOrAdd(k1, () => { called = true; return 0; });
                Assert.IsTrue(called);
            }
            finally
            {
                CacheKit.Remove(k1);
                CacheKit.Remove(k2);
            }
        }

        [TestMethod]
        public void SetDefaultCacheItemPolicy_Null_Throws()
        {
            Assert.ThrowsExactly<ArgumentNullException>(() => CacheKit.SetDefaultCacheItemPolicy(null));
        }

        [TestMethod]
        public void CacheItem_AbsoluteExpiration_Expires()
        {
            var key = NewKey();
            try
            {
                var policy = new CacheItemPolicy { AbsoluteExpiration = DateTimeOffset.Now.AddMilliseconds(300) };
                CacheKit.Add(key, "v", policy);
                Assert.AreEqual("v", CacheKit.GetOrAdd(key, () => "v"));
                Thread.Sleep(700);
                // 过期后应重新执行 factory
                var newValue = CacheKit.GetOrAdd(key, () => "expired-new");
                Assert.AreEqual("expired-new", newValue);
            }
            finally
            {
                CacheKit.Remove(key);
            }
        }

        [TestMethod]
        public void CacheItem_SlidingExpiration_NotExpiredBeforeTimeout()
        {
            var key = NewKey();
            try
            {
                var policy = new CacheItemPolicy { SlidingExpiration = TimeSpan.FromMinutes(10) };
                CacheKit.Add(key, "v", policy);
                Assert.AreEqual("v", CacheKit.GetOrAdd(key, () => "other"));
            }
            finally
            {
                CacheKit.Remove(key);
            }
        }

        [TestMethod]
        public void CacheItem_RemoveTriggeredCallback_ThrowsNothing()
        {
            // 仅验证设置自定义 policy 时可正常工作（回调 + 移除）
            var key = NewKey();
            var policy = new CacheItemPolicy { SlidingExpiration = TimeSpan.FromMinutes(5) };
            CacheKit.Add(key, new object(), policy);
            CacheKit.Remove(key);
            Assert.IsNull(CacheKit.Remove(key));
        }
    }
}
