using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;

namespace SegaCyrus.ToolKits.Kit
{
    /// <summary>
    /// 通用帮助类
    /// </summary>
    public class CommonKit
    {
        /// <summary>
        /// 执行指定逻辑并忽略异常信息
        /// </summary>
        /// <typeparam name="T">返回类型</typeparam>
        /// <param name="func">待执行逻辑</param>
        /// <param name="defaultValue">异常时默认值</param>
        /// <returns>结果</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T ExecuteWithoutException<T>(Func<T> func, T defaultValue = default(T))
        {
            try
            {
                return func.Invoke();
            }
            catch
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// 执行指定逻辑并忽略异常信息
        /// </summary>
        /// <param name="action">待执行逻辑</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ExecuteWithoutException<T>(Action action)
        {
            try
            {
                action.Invoke();
            }
            catch
            {
            }
        }

        /// <summary>
        /// 获取调用栈信息
        /// </summary>
        /// <param name="index">从第几层开始获取</param>
        /// <returns></returns>
        public static string CallTrace(int index = 0)
        {
            index++;
            var stack = new StackTrace(true);
            var stackFrames = stack.GetFrames();
            var t = stackFrames.Select((r, i) =>
            {
                if (i == 0) return null;
                var m = r.GetMethod();
                return $"{m.DeclaringType.FullName}.{m.Name}\n";
            }).Where(r => !string.IsNullOrWhiteSpace(r)).Skip(index).Reverse().ToList();
            for (var i = 0; i < t.Count; ++i)
                t[i] = "->" + new string(' ', i) + t[i];
            return string.Join(string.Empty, t);
        }

        /// <summary>
        /// 写JSON到指定文件
        /// </summary>
        /// <typeparam name="T">内容的强对象</typeparam>
        /// <param name="path">路径</param>
        /// <param name="content">内容</param>
        public static void WriteJSONFile<T>(string path, T content)
        {
            AssertKit.AssertNotEmpty(path, nameof(path));
            System.IO.File.WriteAllText(path, SerializeKit.GetFormatJSON(content));
        }

        /// <summary>
        /// 写XML到指定文件
        /// </summary>
        /// <typeparam name="T">内容的强对象</typeparam>
        /// <param name="path">路径</param>
        /// <param name="content">内容</param>
        public static void WriteXMLFile<T>(string path, T content)
        {
            AssertKit.AssertNotEmpty(path, nameof(path));
            System.IO.File.WriteAllText(path, SerializeKit.GetFormatXML(content));
        }

        /// <summary>
        /// 从指定文件读取JSON并反序列化强对象
        /// </summary>
        /// <typeparam name="T">内容的强对象</typeparam>
        /// <param name="path">路径</param>
        public static T ReadJSONFile<T>(string path)
        {
            AssertKit.AssertNotEmpty(path, nameof(path));
            var json = System.IO.File.ReadAllText(path);
            return SerializeKit.DeserializeJSON<T>(json);
        }

        /// <summary>
        /// 从指定文件读取XML并反序列化强对象
        /// </summary>
        /// <typeparam name="T">内容的强对象</typeparam>
        /// <param name="path">路径</param>
        public static T ReadXMLFile<T>(string path)
        {
            AssertKit.AssertNotEmpty(path, nameof(path));
            var xml = System.IO.File.ReadAllText(path);
            return SerializeKit.DeserializeXML<T>(xml);
        }

    }
}