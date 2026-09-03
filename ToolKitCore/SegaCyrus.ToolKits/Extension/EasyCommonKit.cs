using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SegaCyrus.ToolKits.Kit;

namespace SegaCyrus.ToolKits.Extension.CommonExtension
{
    /// <summary>
    /// 通用扩展包
    /// </summary>
    public static class EasyCommonKit
    {
        /// <summary>
        /// 字节串转内存流
        /// </summary>
        /// <param name="buffer">字节串</param>
        /// <returns>内存流</returns>
        public static Stream ToStream(this byte[] buffer)
        {
            if (buffer == null)
                return null;
            var stream = new MemoryStream(buffer);
            stream.Seek(0, SeekOrigin.Begin);
            return stream;
        }

        /// <summary>
        /// 将目标数据拆分成每块大小为capactity的数据
        /// </summary>
        /// <typeparam name="T">拆分数据元素类型</typeparam>
        /// <param name="source">拆分数据</param>
        /// <param name="capacity">拆分大小</param>
        /// <returns>拆分结果</returns>
        private static IEnumerable<IEnumerable<T>> Partition<T>(IEnumerable<T> source, int capacity)
        {
            var res = new List<IEnumerable<T>>();
            if (source.NotEmpty())
            {
                var totalPage = (int)Math.Ceiling(source.Count() / (double)capacity);
                for (var currentPage = 0; currentPage < totalPage; ++currentPage)
                    res.Add(source.Skip(currentPage * capacity).Take(capacity).ToArray());
            }

            return res;
        }

        /// <summary>
        /// 将目标数据拆分成每块大小为capactity的数据后执行action
        /// </summary>
        /// <typeparam name="T">拆分数据元素类型</typeparam>
        /// <param name="source">拆分数据</param>
        /// <param name="capacity">拆分大小</param>
        /// <param name="action">执行动作</param>
        public static void Partition<T>(this IEnumerable<T> source, int capacity, Action<IEnumerable<T>> action)
        {
            if (source == null)
                return;
            foreach (var partition in Partition(source, capacity))
                action(partition);
        }

        /// <summary>
        /// 判断数组是否不为空
        /// </summary>
        /// <typeparam name="T">判定目标元素类型</typeparam>
        /// <param name="source">判定目标</param>
        /// <returns>判定结果</returns>
        public static bool NotEmpty<T>(this IEnumerable<T> source)
        {
            return source?.Any() ?? false;
        }

        /// <summary>
        /// 判断数组是否为空
        /// </summary>
        /// <typeparam name="T">判定目标元素类型</typeparam>
        /// <param name="source">判定目标</param>
        /// <returns>判定结果</returns>
        /// 
        public static bool IsEmpty<T>(this IEnumerable<T> source)
        {
            return source == null || !source.Any();
        }

        /// <summary>
        /// 转HashSet
        /// </summary>
        /// <typeparam name="TSource">带转换数据类型</typeparam>
        /// <param name="source">带转换数据</param>
        /// <param name="comparer">比较器</param>
        /// <returns></returns>
        public static HashSet<TSource> ToHashSet<TSource>(this IEnumerable<TSource> source,
            IEqualityComparer<TSource> comparer = null)
        {
            AssertKit.AssertNotNull(source, nameof(source));
            return new HashSet<TSource>(source, comparer);
        }
    }
}