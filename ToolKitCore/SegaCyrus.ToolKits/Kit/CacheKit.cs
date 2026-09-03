using System;
using System.Linq;
using System.Runtime.Caching;

namespace SegaCyrus.ToolKits.Kit
{
    /// <summary>
    /// 简单缓存工具包
    /// </summary>
    public class CacheKit
    {
        private static readonly MemoryCache Cache = new MemoryCache("ToolKitsCache");

        private static CacheItemPolicy DefaultPolicy =
            new CacheItemPolicy { SlidingExpiration = new TimeSpan(0, 0, 10, 0) };

        /// <summary>
        /// 设置默认过期策略
        /// </summary>
        /// <param name="item"></param>
        public static void SetDefaultCacheItemPolicy(CacheItemPolicy item)
        {
            AssertKit.AssertNotNull(item, "CacheItemPolicy");
            DefaultPolicy = item;
        }

        /// <summary>
        /// 获取或添加缓存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="data"></param>
        /// <param name="policy"></param>
        /// <returns></returns>
        public static T GetOrAdd<T>(string key, Func<T> data, CacheItemPolicy policy = null)
        {
            if (!Cache.Contains(key))
                Add(key, data(), policy);
            return (T)Cache.Get(key);
        }

        /// <summary>
        /// 清空所有缓存
        /// </summary>
        public static void Clear()
        {
            var cacheKeys = Cache.Select(kvp => kvp.Key).ToList();
            foreach (var cacheKey in cacheKeys)
                Remove(cacheKey);
        }

        /// <summary>
        /// 添加指定缓存
        /// </summary>
        /// <param name="key">缓存KEY</param>
        /// <param name="obj">缓存VALUE</param>
        /// <param name="policy">缓存过期策略</param>
        /// <returns></returns>
        public static bool Add(string key, object obj, CacheItemPolicy policy = null)
            => Cache.Add(new CacheItem(key, obj), policy ?? DefaultPolicy);

        /// <summary>
        /// 移除指定缓存
        /// </summary>
        /// <param name="key">缓存KEY</param>
        /// <returns></returns>
        public static object Remove(string key) => Cache.Remove(key);
    }
}