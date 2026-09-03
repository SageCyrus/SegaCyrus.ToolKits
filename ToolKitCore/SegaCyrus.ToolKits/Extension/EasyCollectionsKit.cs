using System;
using System.Collections.Generic;
using System.Linq;

namespace SegaCyrus.ToolKits.Extension.CollectionsExtension
{
    /// <summary>
    /// 集合扩展
    /// </summary>
    public static class EasyCollectionsKit
    {
        private static T ConflictSelect<T>(T l, T r) => l;

        /// <summary>
        /// 合并两个字典
        /// </summary>
        /// <typeparam name="TKey">字典key类型</typeparam>
        /// <typeparam name="TValue">字典value类型</typeparam>
        /// <param name="src">来源字典</param>
        /// <param name="r">并入字典</param>
        /// <param name="conflictSelector">冲突选择器 默认逻辑保留来源字典</param>
        /// <returns>合并结果</returns>
        public static Dictionary<TKey, TValue> MergeDictionary<TKey, TValue>(this Dictionary<TKey, TValue> src, Dictionary<TKey, TValue> r, Func<TValue, TValue, TValue> conflictSelector = null)
        {
            conflictSelector = conflictSelector ?? ConflictSelect;
            src = src ?? new Dictionary<TKey, TValue>();
            r = r ?? new Dictionary<TKey, TValue>();
            var result = new Dictionary<TKey, TValue>(src.Count() + r.Count());
            foreach (var item in src)
                result[item.Key] = item.Value;
            foreach (var item in r)
                result[item.Key] = result.TryGetValue(item.Key, out var value) ? conflictSelector(value, item.Value) : item.Value;
            return result;
        }
    }
}