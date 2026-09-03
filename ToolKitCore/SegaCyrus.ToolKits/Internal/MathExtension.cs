using System;
using System.Collections.Generic;
using SegaCyrus.ToolKits.Extension.CommonExtension;

namespace SegaCyrus.ToolKits.Internal
{
    /// <summary>
    /// 数学扩展包
    /// </summary>
    internal static class MathExtension
    {
        /// <summary>
        /// 矩阵转置
        /// </summary>
        /// <param name="matrix">矩阵</param>
        /// <typeparam name="T">T</typeparam>
        /// <returns></returns>
        public static List<List<T>> Transpose<T>(List<List<T>> matrix)
        {
            if (matrix.IsEmpty())
                return new List<List<T>>();

            var maxColumnCount = 0;
            foreach (var item in matrix)
                maxColumnCount = Math.Max(maxColumnCount, item.Count);
            var result = new List<List<T>>(maxColumnCount);
            for (var i = 0; i < maxColumnCount; i++)
                result.Add(new List<T>(matrix.Count));

            for (var i = 0; i < matrix.Count; i++)
            {
                var j = 0;
                var item = matrix[i];
                for (; j < item.Count; ++j)
                    result[j].Add(item[j]);
                for (; j < maxColumnCount; ++j)
                    result[j].Add(default(T));
            }

            return result;
        }
    }
}