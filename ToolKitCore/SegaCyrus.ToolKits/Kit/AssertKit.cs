using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using SegaCyrus.ToolKits.Extension.CommonExtension;

namespace SegaCyrus.ToolKits.Kit
{
    /// <summary>
    /// 断言工具包
    /// </summary>
    public class AssertKit
    {
        /// <summary>
        /// 校验参数必须为TRUE
        /// </summary>
        /// <param name="data">参数</param>
        /// <param name="paramName">参数名</param>
        /// <param name="msg">错误提示</param>
        /// <returns>参数</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AssertTrue(bool data, string paramName = "", string msg = null)
        {
            return AssertFalse(data == false, paramName, msg ?? $"{paramName}参数必须为True");
        }

        /// <summary>
        /// 校验参数必须为False
        /// </summary>
        /// <param name="data">参数</param>
        /// <param name="paramName">参数名</param>
        /// <param name="msg">错误提示</param>
        /// <returns>参数</returns>
        /// <exception cref="ArgumentException">校验失败</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AssertFalse(bool data, string paramName = "", string msg = null)
        {
            if (data)
                throw new ArgumentException(msg ?? "参数必须为False");
            return data;
        }

        /// <summary>
        /// 校验参数必须大于0
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data">数据</param>
        /// <param name="paramName">参数名,用于构造错误提示</param>
        /// <param name="msg">重写错误提示</param>
        /// <returns>参数</returns>
        /// <exception cref="ArgumentOutOfRangeException">校验失败后的异常信息</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T AssertPositive<T>(T data, string paramName = "", string msg = null) where T : IComparable
        {
            if (data.CompareTo(0) <= 0)
                throw new ArgumentOutOfRangeException(msg ?? $"参数{paramName}值为{data},不满足必须为正数的约束");
            return data;
        }


        /// <summary>
        /// 校验参数在指定范围
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="compare">比较基准值</param>
        /// <param name="paramName">参数名,用于构造错误提示</param>
        /// <param name="msg">重写错误提示</param>
        /// <returns>参数</returns>
        /// <exception cref="ArgumentOutOfRangeException">校验失败后的异常信息</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T AssertGreater<T>(T data, T compare, string paramName = "", string msg = null)
            where T : IComparable
        {
            if (data.CompareTo(compare) <= 0)
                throw new ArgumentOutOfRangeException(msg ?? $"参数{paramName}值为{data},不满足约束大于{compare}");
            return data;
        }

        /// <summary>
        /// 校验参数在指定范围
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="compare">比较基准值</param>
        /// <param name="paramName">参数名,用于构造错误提示</param>
        /// <param name="msg">重写错误提示</param>
        /// <returns>参数</returns>
        /// <exception cref="ArgumentOutOfRangeException">校验失败后的异常信息</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T AssertLesser<T>(T data, T compare, string paramName = "", string msg = null)
            where T : IComparable
        {
            if (data.CompareTo(compare) >= 0)
                throw new ArgumentOutOfRangeException(msg ?? $"参数{paramName}值为{data},不满足约束大于{compare}");
            return data;
        }

        /// <summary>
        /// 校验参数在指定范围
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="compare">比较基准值</param>
        /// <param name="paramName">参数名,用于构造错误提示</param>
        /// <param name="msg">重写错误提示</param>
        /// <returns>参数</returns>
        /// <exception cref="ArgumentOutOfRangeException">校验失败后的异常信息</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T AssertEQGreater<T>(T data, T compare, string paramName = "", string msg = null)
            where T : IComparable
        {
            if (data.CompareTo(compare) < 0)
                throw new ArgumentOutOfRangeException(msg ?? $"参数{paramName}值为{data},不满足约束大于{compare}");
            return data;
        }

        /// <summary>
        /// 校验参数在指定范围
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="compare">比较基准值</param>
        /// <param name="paramName">参数名,用于构造错误提示</param>
        /// <param name="msg">重写错误提示</param>
        /// <returns>参数</returns>
        /// <exception cref="ArgumentOutOfRangeException">校验失败后的异常信息</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T AssertEQLesser<T>(T data, T compare, string paramName = "", string msg = null)
            where T : IComparable
        {
            if (data.CompareTo(compare) > 0)
                throw new ArgumentOutOfRangeException(msg ?? $"参数{paramName}值为{data},不满足约束大于{compare}");
            return data;
        }

        /// <summary>
        /// 校验参数在指定范围
        /// </summary>
        /// <param name="checkCount">参数</param>
        /// <param name="min">最小值</param>
        /// <param name="max">最大值</param>
        /// <param name="paramName">参数名,用于构造错误提示</param>
        /// <param name="msg">重写错误提示</param>
        /// <returns>参数</returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T AssertRange<T>(T checkCount, T min, T max, string paramName = "", string msg = null)
            where T : IComparable
        {
            if (checkCount.CompareTo(min) < 0 || checkCount.CompareTo(max) > 0)
                throw new ArgumentOutOfRangeException(msg ?? $"参数{paramName}值为{checkCount},不满足约束大于等于{min}或小于等于{max}");
            return checkCount;
        }

        /// <summary>
        /// 校验参数不为空
        /// </summary>
        /// <typeparam name="T">参数类型</typeparam>
        /// <param name="arg">参数</param>
        /// <param name="paramName">参数名,用于构造错误提示</param>
        /// <param name="msg">重写错误提示</param>
        /// <returns>参数</returns>
        /// <exception cref="ArgumentNullException"></exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T AssertNotNull<T>(T arg, string paramName = "", string msg = null)
        {
            if (arg == null)
                throw new ArgumentNullException(msg ?? $"参数{paramName}值为空,不满足约束非空");
            return arg;
        }

        /// <summary>
        /// 校验参数不为空并且存在至少一个元素
        /// </summary>
        /// <typeparam name="T">参数类型</typeparam>
        /// <param name="args">参数</param>
        /// <param name="paramName">参数名,用于构造错误提示</param>
        /// <param name="msg">重写错误提示</param>
        /// <returns>参数</returns>
        /// <exception cref="ArgumentNullException"></exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<T> AssertNotEmpty<T>(IEnumerable<T> args, string paramName = "", string msg = null)
        {
            if (args.NotEmpty())
                return args;
            throw new ArgumentException(msg ?? $"参数{paramName}为空或者不存在元素,不满足约束");
        }

        /// <summary>
        /// 校验参数不为空
        /// </summary>
        /// <param name="args">参数</param>
        /// <param name="paramName">参数名,用于构造错误提示</param>
        /// <param name="msg">重写错误提示</param>
        /// <returns>参数</returns>
        /// <exception cref="ArgumentNullException"></exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string AssertNotEmpty(string args, string paramName = "", string msg = null)
        {
            if (string.IsNullOrEmpty(args) == false)
                return args;
            throw new ArgumentException(msg ?? $"参数{paramName}为空,不满足约束");
        }
    }
}