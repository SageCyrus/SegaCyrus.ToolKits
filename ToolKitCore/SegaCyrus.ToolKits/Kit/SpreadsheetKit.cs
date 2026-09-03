using System.Collections.Generic;
using System.Data;
using SegaCyrus.ToolKits.Model.SpreadSheet.Append;
using SegaCyrus.ToolKits.Model.SpreadSheet.Read;
using SegaCyrus.ToolKits.Model.SpreadSheet.Write;
using SegaCyrus.ToolKits.Package.SpreadSheetPackage;

namespace SegaCyrus.ToolKits.Kit
{
    /// <summary>
    /// 电子表格工具包
    /// </summary>
    /// <remarks>
    /// 如需新增策略实现器
    /// 可以继承<see cref="SegaCyrus.ToolKits.Package.SpreadSheetPackage.Base.SpreadSheetPolicy"/>类
    /// 并标记特性<see cref="SegaCyrus.ToolKits.Model.SpreadSheet.Attributes.SpreadSheetRegisterAttribute"/>
    /// <code>
    /// [SpreadSheetRegister(Key)]
    /// class MySelfPolicy:SegaCyrus.ToolKits.Package.SpreadSheetPackage.Base.SpreadSheetPolicy { }
    /// </code>
    /// 在工厂初始化的时候会自动构造实例
    /// 在需要的时候使用工厂获取实例
    /// <code>
    ///  SpreadSheetProxy.Instance.GetPolicy(Key).WriteSpreadSheet(configArgs, data);
    /// </code>
    /// 如有必要可调用方法SpreadSheetProxy.Instance.ReplacePolicy替换或新增策略实现类
    /// <code>
    ///  SpreadSheetProxy.Instance.ReplacePolicy(Key,new MySelfPolicy())
    /// </code>
    /// </remarks>
    public class SpreadsheetKit
    {
        #region Write

        /// <summary>
        /// 电子表格写入
        /// </summary>
        /// <typeparam name="TKey">Key Type</typeparam>
        /// <typeparam name="TValue">Value Type</typeparam>
        /// <param name="configArgs">
        /// 配置参数
        /// <seealso cref="WriteCsvConfigSpreadSheetArgs"/>
        /// <seealso cref="WriteExcelConfigSpreadSheetArgs"/>
        /// <seealso cref="WriteStringConfigSpreadSheetArgs"/>
        /// </param>
        /// <param name="data">转换数据</param>
        /// <returns>转换结果</returns>
        /// <remarks>
        /// 根据参数<paramref name="configArgs"/>决定返回值
        /// 当参数为<seealso cref="WriteCsvConfigSpreadSheetArgs"/>时 返回值类型为string
        /// 当参数为<seealso cref="WriteStringConfigSpreadSheetArgs"/>时 返回值类型为string
        /// 当参数为<seealso cref="WriteExcelConfigSpreadSheetArgs"/>时 返回值类型为bytes[]
        /// </remarks>
        public static object WriteSpreadSheet<TKey, TValue>(WriteConfigSpreadSheetArgs configArgs,
            IEnumerable<Dictionary<TKey, TValue>> data)
#if NET_8
            where TKey notnul;
#else
            where TKey : class
#endif
        {
            return SpreadSheetProxy.Instance.GetPolicy(configArgs.TypeEnum).WriteSpreadSheet(configArgs, data);
        }

        /// <summary>
        /// 电子表格写入
        /// </summary>
        /// <typeparam name="TData">数据类型</typeparam>
        /// <param name="configArgs">
        /// 配置参数
        /// <seealso cref="WriteCsvConfigSpreadSheetArgs"/>
        /// <seealso cref="WriteExcelConfigSpreadSheetArgs"/>
        /// <seealso cref="WriteStringConfigSpreadSheetArgs"/>
        /// </param>
        /// <param name="data">转换数据</param>
        /// <returns>转换结果</returns>
        /// <remarks>
        /// 根据参数<paramref name="configArgs"/>决定返回值
        /// 当参数为<seealso cref="WriteCsvConfigSpreadSheetArgs"/>时 返回值类型为string
        /// 当参数为<seealso cref="WriteStringConfigSpreadSheetArgs"/>时 返回值类型为string
        /// 当参数为<seealso cref="WriteExcelConfigSpreadSheetArgs"/>时 返回值类型为bytes[]
        /// </remarks>
        public static object WriteSpreadSheet<TData>(WriteConfigSpreadSheetArgs configArgs, IEnumerable<TData> data)
#if NET_8
            where TData notnul;
#else
            where TData : class
#endif
        {
            return SpreadSheetProxy.Instance.GetPolicy(configArgs.TypeEnum).WriteSpreadSheet(configArgs, data);
        }

        /// <summary>
        /// 电子表格写入
        /// </summary>
        /// <param name="configArgs">
        /// 配置参数
        /// <seealso cref="WriteCsvConfigSpreadSheetArgs"/>
        /// <seealso cref="WriteExcelConfigSpreadSheetArgs"/>
        /// <seealso cref="WriteStringConfigSpreadSheetArgs"/>
        /// </param>
        /// <param name="data">转换数据</param>
        /// <returns>转换结果</returns>
        /// <remarks>
        /// 根据参数<paramref name="configArgs"/>决定返回值
        /// 当参数为<seealso cref="WriteCsvConfigSpreadSheetArgs"/>时 返回值类型为string
        /// 当参数为<seealso cref="WriteStringConfigSpreadSheetArgs"/>时 返回值类型为string
        /// 当参数为<seealso cref="WriteExcelConfigSpreadSheetArgs"/>时 返回值类型为bytes[]
        /// </remarks>
        public static object WriteSpreadSheet(WriteConfigSpreadSheetArgs configArgs, DataTable data)
        {
            return SpreadSheetProxy.Instance.GetPolicy(configArgs.TypeEnum).WriteSpreadSheet(configArgs, data);
        }

        #endregion

        /// <summary>
        /// 电子表格读取
        /// </summary>
        /// <typeparam name="TData">数据类型</typeparam>
        /// <param name="args">
        /// 配置参数
        /// <seealso cref="WriteCsvConfigSpreadSheetArgs"/>
        /// <seealso cref="WriteExcelConfigSpreadSheetArgs"/>
        /// <seealso cref="WriteStringConfigSpreadSheetArgs"/>
        /// </param>
        /// <returns>返回对应的强模型</returns>
        public static List<TData> ReadModel<TData>(ReadSpreadSheetArgs args)
#if NET_8
            where TData notnul;
#else
            where TData : class
#endif
        {
            return SpreadSheetProxy.Instance.GetPolicy(args.TypeEnum).ReadModel<TData>(args);
        }

        /// <summary>
        /// 电子表格读取
        /// </summary>
        /// <param name="args">
        /// 配置参数
        /// <seealso cref="WriteCsvConfigSpreadSheetArgs"/>
        /// <seealso cref="WriteExcelConfigSpreadSheetArgs"/>
        /// <seealso cref="WriteStringConfigSpreadSheetArgs"/>
        /// </param>
        /// <returns>返回对应的动态类型, 且所有属性均为String类型</returns>
        public static List<dynamic> ReadDynamic(ReadSpreadSheetArgs args)
        {
            return SpreadSheetProxy.Instance.GetPolicy(args.TypeEnum).ReadDynamic(args);
        }

        /// <summary>
        /// 电子表格读取
        /// </summary>
        /// <param name="args">
        /// 配置参数
        /// <seealso cref="WriteCsvConfigSpreadSheetArgs"/>
        /// <seealso cref="WriteExcelConfigSpreadSheetArgs"/>
        /// <seealso cref="WriteStringConfigSpreadSheetArgs"/>
        /// </param>
        /// <returns>返回对应的动态类型, 对应属性为此列推断后的数据</returns>
        public static List<dynamic> ReadDynamicAutoType(ReadSpreadSheetArgs args)
        {
            return SpreadSheetProxy.Instance.GetPolicy(args.TypeEnum).ReadDynamicAutType(args);
        }

        /// <summary>
        /// 电子表格读取
        /// </summary>
        /// <param name="args">
        /// 配置参数
        /// <seealso cref="WriteCsvConfigSpreadSheetArgs"/>
        /// <seealso cref="WriteExcelConfigSpreadSheetArgs"/>
        /// <seealso cref="WriteStringConfigSpreadSheetArgs"/>
        /// </param>
        /// <returns>返回对应的动态类型, 且所有属性均为String类型</returns>
        public static List<Dictionary<string, string>> ReadDict(ReadSpreadSheetArgs args)
        {
            return SpreadSheetProxy.Instance.GetPolicy(args.TypeEnum).ReadDict(args);
        }

        /// <summary>
        /// 电子表格读取
        /// </summary>
        /// <param name="args">
        /// 配置参数
        /// <seealso cref="WriteCsvConfigSpreadSheetArgs"/>
        /// <seealso cref="WriteExcelConfigSpreadSheetArgs"/>
        /// <seealso cref="WriteStringConfigSpreadSheetArgs"/>
        /// </param>
        /// <returns>返回对应的动态类型, 对应属性为此列推断后的数据</returns>
        public static List<Dictionary<string, dynamic>> ReadDictAutoType(ReadSpreadSheetArgs args)
        {
            return SpreadSheetProxy.Instance.GetPolicy(args.TypeEnum).ReadDictAutoType(args);
        }


        #region AppendData

        /// <summary>
        /// 写入到指定句柄
        /// 如写入多次,对于数据源下拉Sheet名之前写入过,那本次跳过写入并使用之前的数据源校验信息,反之则新增
        /// 如写入多次,对于已经写入过的数据Sheet页名,在之前写入过的Sheet页内新增,反之则新增
        /// </summary>
        /// <typeparam name="TKey">Key Type</typeparam>
        /// <typeparam name="TValue">Value Type</typeparam>
        /// <param name="configArgs">写入配置 配置参数
        /// <seealso cref="WriteCsvConfigSpreadSheetArgs"/>
        /// <seealso cref="WriteExcelConfigSpreadSheetArgs"/>
        /// <seealso cref="WriteStringConfigSpreadSheetArgs"/>
        /// </param>
        /// <param name="data">目标数据</param>
        /// <param name="handle">句柄,如果为空则构造新的句柄</param>
        /// <returns>句柄</returns>
        public static WriteSpreadSheetHandle WriteSpreadSheetHandle<TKey, TValue>(WriteConfigSpreadSheetArgs configArgs,
            IEnumerable<Dictionary<TKey, TValue>> data, WriteSpreadSheetHandle handle = null)
#if NET_8
            where TKey notnul;
#else
            where TKey : class
#endif
        {
            return SpreadSheetProxy.Instance.GetPolicy(configArgs.TypeEnum)
                .WriteSpreadSheetHandle(configArgs, data, handle);
        }

        /// <summary>
        /// 写入到指定句柄
        /// 如写入多次,对于数据源下拉Sheet名之前写入过,那本次跳过写入并使用之前的数据源校验信息,反之则新增
        /// 如写入多次,对于已经写入过的数据Sheet页名,在之前写入过的Sheet页内新增,反之则新增
        /// </summary>
        /// <typeparam name="TData">元素数据</typeparam>  
        /// <param name="configArgs">写入配置 配置参数
        /// <seealso cref="WriteCsvConfigSpreadSheetArgs"/>
        /// <seealso cref="WriteExcelConfigSpreadSheetArgs"/>
        /// <seealso cref="WriteStringConfigSpreadSheetArgs"/>
        /// </param>
        /// <param name="data">目标数据</param>
        /// <param name="handle">句柄,如果为空则构造新的句柄</param>
        /// <returns>句柄</returns>
        public static WriteSpreadSheetHandle WriteSpreadSheetHandle<TData>(WriteConfigSpreadSheetArgs configArgs,
            IEnumerable<TData> data, WriteSpreadSheetHandle handle = null)
#if NET_8
            where TData notnul;
#else
            where TData : class
#endif
        {
            return SpreadSheetProxy.Instance.GetPolicy(configArgs.TypeEnum)
                .WriteSpreadSheetHandle(configArgs, data, handle);
        }

        /// <summary>
        /// 写入到指定句柄
        /// 如写入多次,对于数据源下拉Sheet名之前写入过,那本次跳过写入并使用之前的数据源校验信息,反之则新增
        /// 如写入多次,对于已经写入过的数据Sheet页名,在之前写入过的Sheet页内新增,反之则新增
        /// </summary>
        /// <param name="configArgs">写入配置 配置参数
        /// <seealso cref="WriteCsvConfigSpreadSheetArgs"/>
        /// <seealso cref="WriteExcelConfigSpreadSheetArgs"/>
        /// <seealso cref="WriteStringConfigSpreadSheetArgs"/>
        /// </param>
        /// <param name="data">目标数据</param>
        /// <param name="handle">句柄,如果为空则构造新的句柄</param>
        /// <returns>句柄</returns>
        public static WriteSpreadSheetHandle WriteSpreadSheetHandle(WriteConfigSpreadSheetArgs configArgs,
            DataTable data, WriteSpreadSheetHandle handle = null)
        {
            return SpreadSheetProxy.Instance.GetPolicy(configArgs.TypeEnum)
                .AppendSpreadSheetData(configArgs, data, handle);
        }

        #endregion
    }
}