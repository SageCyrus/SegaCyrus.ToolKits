using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using SegaCyrus.ToolKits.Extension.CommonExtension;
using SegaCyrus.ToolKits.Model.Common;
using SegaCyrus.ToolKits.Model.Console;
using SegaCyrus.ToolKits.Model.Console.Attributes;
using SegaCyrus.ToolKits.Model.SpreadSheet.Write;

namespace SegaCyrus.ToolKits.Kit
{
    /// <summary>
    /// 控制台工具包
    /// </summary>
    public class ConsoleKit
    {
        /// <summary>
        /// 输出一段提示，等待用户输入非空字符串
        /// </summary>
        /// <param name="input">提示语</param>
        /// <param name="noticColor">颜色</param>
        /// <param name="errorColor">报错颜色</param>
        /// <param name="withTime">是否打印时间</param>
        /// <returns>用户输入后的字符串</returns>
        public static string WaitInput(object input, ConsoleColor noticColor = ConsoleColor.Cyan,
            ConsoleColor errorColor = ConsoleColor.Red, bool withTime = true)
        {
            WriteConsole(input, noticColor, withTime);
            while (true)
            {
                var key = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(key))
                    return key;
                WriteConsole("输入不正确,请重新输入", errorColor);
            }
        }

        /// <summary>
        /// 等待用户输入指定数据
        /// </summary>
        /// <param name="input">等待输入的数据</param>
        /// <param name="noticColor">颜色</param>
        /// <param name="errorColor">报错颜色</param>
        /// <param name="withTime">是否追加时间</param>
        public static void WaitConfirmInput(object input, ConsoleColor noticColor = ConsoleColor.Cyan,
            ConsoleColor errorColor = ConsoleColor.Red, bool withTime = true)
        {
            WriteConsole($"请输入{input}以继续", noticColor, withTime: withTime);
            while (true)
            {
                var key = Console.ReadLine();
                if (key == input?.ToString())
                    break;
                WriteConsole("输入不正确,请重新输入", errorColor, withTime: withTime);
            }
        }

        /// <summary>
        /// 输出一段信息到控制台
        /// </summary>
        /// <param name="msg">输出内容</param>
        /// <param name="color">颜色</param>
        /// <param name="withTime">是否携带时间</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteConsole(object msg, ConsoleColor color = ConsoleColor.Red, bool withTime = true)
        {
            InternalWriteConsole(msg, DateTime.Now, color, withTime);
            //AsyncTaskKit.AddTaskCore(new AsyncConsoleLogMsg(msg, color, withTime));
        }

        /// <summary>
        /// 输出一段信息到控制台
        /// </summary>
        /// <param name="msg">输出内容</param>
        /// <param name="time">时间</param>
        /// <param name="color">颜色</param>
        /// <param name="withTime">是否携带时间</param>
        internal static void InternalWriteConsole(object msg, DateTime time, ConsoleColor color = ConsoleColor.Red,
            bool withTime = true)
        {
            var foregroundColor = (int)Console.ForegroundColor;
            Console.ForegroundColor = color;
            if (withTime)
                Console.WriteLine(time.ToString("yyyy/MM/dd HH:mm:ss") + ": " + msg);
            else
                Console.WriteLine(msg);
            Console.ForegroundColor = (ConsoleColor)foregroundColor;
        }


        /// <summary>
        /// 解析命令行参数
        /// </summary>
        /// <typeparam name="T">解析到目标强对象</typeparam>
        /// <param name="args">输入的参数,一般是Main函数的args</param>
        /// <param name="helpContent">重写帮助提示</param>
        /// <param name="checkRequire">校验必填参数</param>
        /// <returns></returns>
        public static T ParseArgs<T>(string[] args, string helpContent = null, bool checkRequire = true)
        {
            var attributeType = typeof(CommandPropertyAttribute);
            var result = ReflectKit.CreateInstanceWithDefaultValue<T>();

            var property = typeof(T).GetProperties()
                .Select(c =>
                    new KVModel<CommandPropertyAttribute, PropertyInfo>(
                        (CommandPropertyAttribute)c.GetCustomAttribute(attributeType, true), c))
                .Where(c => c.Key != null)
                .ToList();

            if (property.IsEmpty())
                return result;

            var tmp = property.SelectMany(c =>
                    new List<KVModel<string, KVModel<CommandPropertyAttribute, PropertyInfo>>>()
                    {
                        new KVModel<string, KVModel<CommandPropertyAttribute, PropertyInfo>>(
                            c.Key.IsCaseSensitive ? c.Key.ShortName + "_CaseSensitive" : c.Key.ShortName?.ToLower(), c),
                        new KVModel<string, KVModel<CommandPropertyAttribute, PropertyInfo>>(
                            c.Key.IsCaseSensitive ? c.Key.FullName + "_CaseSensitive" : c.Key.FullName?.ToLower(), c)
                    })
                .Where(c => false == string.IsNullOrEmpty(c.Key));
            AssertKit.AssertTrue(tmp.Count() == tmp.Select(c => c.Key).ToHashSet().Count(), "存在相同key");
            var propertyMap = tmp.ToDictionary(c => $"-{c.Key}", c => c.Value);

            var requirePool = property.Where(c => c.Key.IsRequire).Select(c => c.Value.Name).ToHashSet();

            for (var i = 0; i < args.Length; ++i)
            {
#if NET8_0_OR_GREATER
                if (args[i].StartsWith('-'))
#else
                if (args[i].StartsWith("-"))
#endif
                {
                    KVModel<CommandPropertyAttribute, PropertyInfo> configData = null;
                    var compareStr = args[i].ToLower();
                    if (propertyMap.TryGetValue(compareStr, out configData) ||
                        propertyMap.TryGetValue(args[i] + "_CaseSensitive", out configData))
                    {
                        var fieldInfo = configData.Value;
                        requirePool.Remove(fieldInfo.Name);
                        if (ReflectKit.IsList(fieldInfo.PropertyType))
                        {
                            var arrList = new List<string>();
                            for (i = i + 1; i < args.Length; ++i)
                            {
                                if (!args[i].StartsWith("-"))
                                    arrList.Add(args[i]);
                                else
                                {
                                    --i;
                                    break;
                                }
                            }

                            if (fieldInfo.PropertyType.IsArray)
                            {
                                var instance = (Array)ReflectKit.CreateInstance(fieldInfo.PropertyType,
                                    new object[] { arrList.Count });
                                var convertHandle =
                                    ReflectKit.GetConvertDataHandle(fieldInfo.PropertyType.GetElementType());
                                for (var ii = 0; ii < arrList.Count; ++ii)
                                    instance.SetValue(convertHandle(arrList[ii]), ii);
                                fieldInfo.SetValue(result, instance);
                            }
                            else
                            {
                                var instance = (IList)ReflectKit.CreateInstance(fieldInfo.PropertyType);
                                var convertHandle =
                                    ReflectKit.GetConvertDataHandle(fieldInfo.PropertyType.GenericTypeArguments[0]);
                                foreach (var item in arrList)
                                    instance.Add(convertHandle(item));
                                fieldInfo.SetValue(result, instance);
                            }
                        }
                        else if (fieldInfo.PropertyType.IsGenericType && fieldInfo.PropertyType
                                     .GetGenericTypeDefinition().GetInterfaces()
                                     .FirstOrDefault(c => c.GUID == typeof(ISet<>).GUID) != null)
                        {
                            var instance = ReflectKit.CreateInstance(fieldInfo.PropertyType);
                            var convertHandle =
                                ReflectKit.GetConvertDataHandle(fieldInfo.PropertyType.GenericTypeArguments[0]);
                            var addMethod = fieldInfo.PropertyType.GetMethod("Add",
                                new[] { fieldInfo.PropertyType.GenericTypeArguments[0] });
                            for (i = i + 1; i < args.Length; ++i)
                            {
#if NET8_0_OR_GREATER
                                if (!args[i].StartsWith('-'))
#else
                                if (!args[i].StartsWith("-"))
#endif
                                    addMethod.Invoke(instance, new[] { convertHandle.Invoke(args[i]) });
                                else
                                {
                                    --i;
                                    break;
                                }
                            }

                            fieldInfo.SetValue(result, instance);
                        }

                        else if (fieldInfo.PropertyType == typeof(bool))
                            fieldInfo.SetValue(result, true);
                        else
                        {
                            ++i;
                            fieldInfo.SetValue(result,
                                ReflectKit.GetConvertDataHandle(fieldInfo.PropertyType)(args[i]));
                        }
                    }

                    if (compareStr == "-help" || compareStr == "-h")
                    {
                        if (!string.IsNullOrWhiteSpace(helpContent))
                        {
                            WriteConsole(helpContent, ConsoleColor.Green, withTime: false);
                            return default;
                        }

                        var excelHelper = new List<CommandArgsHelpModel>();
                        foreach (var item in property)
                        {
                            var itemArgsHelp = new CommandArgsHelpModel();

                            itemArgsHelp.CommandArgsPrefix = item.Key.ShortName;
                            if (string.IsNullOrEmpty(item.Key.FullName))
                            {
                                if (string.IsNullOrEmpty(itemArgsHelp.CommandArgsPrefix))
                                    itemArgsHelp.CommandArgsPrefix = item.Key.FullName;
                                else
                                    itemArgsHelp.CommandArgsPrefix += $", {item.Key.FullName}";
                            }

                            itemArgsHelp.IsRequired = item.Key.IsRequire;
                            itemArgsHelp.Descript = item.Key.Descript;

                            itemArgsHelp.IsMulti = ReflectKit.IsList(item.Value.PropertyType)
                                                   ||
                                                   (item.Value.PropertyType.IsGenericType &&
                                                    item.Value.PropertyType.GetGenericTypeDefinition().GetInterfaces()
                                                        .FirstOrDefault(c => c.GUID == typeof(ISet<>).GUID) != null);
                            if (itemArgsHelp.IsMulti)
                            {
                                if (item.Value.PropertyType.IsArray)
                                {
                                    if (item.Value.PropertyType.GetElementType().IsEnum)
                                        itemArgsHelp.ParamPool = string.Join(", ",
                                            Enum.GetNames(item.Value.PropertyType.GetElementType()));
                                }
                                else
                                {
                                    if (item.Value.PropertyType.GenericTypeArguments[0].IsEnum)
                                        itemArgsHelp.ParamPool = string.Join(", ",
                                            Enum.GetNames(item.Value.PropertyType.GenericTypeArguments[0]));
                                }
                            }
                            else if (item.Value.PropertyType.IsEnum)
                                itemArgsHelp.ParamPool = string.Join(", ", Enum.GetNames(item.Value.PropertyType));

                            itemArgsHelp.IsCaseSensitive = item.Key.IsCaseSensitive;
                            excelHelper.Add(itemArgsHelp);
                        }

                        var helpeData =
                            (string)SpreadsheetKit.WriteSpreadSheet(
                                new WriteStringConfigSpreadSheetArgs(true, false, true, 28), excelHelper);
                        WriteConsole(helpeData, ConsoleColor.Green, withTime: false);
                        return default;
                    }
                }
                else
                {
                    throw new ArgumentException($"未知命令,终止操作 {args[i]}");
                }
            }

            if (checkRequire && requirePool.NotEmpty())
                throw new ArgumentException($"参数必填:" + string.Join(",", requirePool));
            return result;
        }
    }
}