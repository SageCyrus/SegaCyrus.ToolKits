using SegaCyrus.ToolKits.Extension.SpreadsheetExtension.EasyWithConfig;
using SegaCyrus.ToolKits.Kit;
using SegaCyrus.ToolKits.Model.SpreadSheet.Append;
using SegaCyrus.ToolKits.Model.SpreadSheet.Read;
using SegaCyrus.ToolKits.Model.SpreadSheet.Write;
using System.Collections.Generic;
using System.Data;

namespace SegaCyrus.ToolKits.Extension.SpreadsheetExtension
{
    /// <summary>
    /// SpreadsheetKit扩展包
    /// </summary>
    public static class EasySpreadsheetKit
    {


        /// <summary>
        /// 写入数据
        /// </summary>
        /// <typeparam name="TKey">写入列名</typeparam>
        /// <typeparam name="TValue">写入行数据</typeparam>
        /// <param name="handle">实例句柄可空</param>
        /// <param name="args">写入配置</param>
        /// <param name="data">写入数据</param>
        /// <returns>写入句柄 若目标句柄为null则创建</returns>
        public static WriteSpreadSheetHandle WriteSpreadSheet<TKey, TValue>(this WriteSpreadSheetHandle handle, WriteConfigSpreadSheetArgs args,IEnumerable<Dictionary<TKey, TValue>> data)
#if NET_8
            where TKey notnul;
#else
            where TKey : class
#endif
        {
            return SpreadsheetKit.WriteSpreadSheetHandle(args, data, handle);
        }

        /// <summary>
        /// 写入数据
        /// </summary>
        /// <typeparam name="TData">写入单个数据类型</typeparam>
        /// <param name="handle">实例句柄可空</param>
        /// <param name="args">写入配置</param>
        /// <param name="data">写入数据</param>
        /// <returns>写入句柄 若目标句柄为null则创建</returns>
        public static WriteSpreadSheetHandle WriteSpreadSheet<TData>(this WriteSpreadSheetHandle handle, WriteConfigSpreadSheetArgs args, IEnumerable<TData> data)
#if NET_8
            where TData notnul;
#else
            where TData : class
#endif
        {
            return SpreadsheetKit.WriteSpreadSheetHandle(args, data, handle);
        }

        /// <summary>
        /// 写入数据
        /// </summary>
        /// <param name="handle">实例句柄可空</param>
        /// <param name="args">写入配置</param>
        /// <param name="data">写入数据</param>
        /// <returns>写入句柄 若目标句柄为null则创建</returns>
        public static WriteSpreadSheetHandle WriteSpreadSheet(this WriteSpreadSheetHandle handle, WriteConfigSpreadSheetArgs args, DataTable data)
        {
            return SpreadsheetKit.WriteSpreadSheetHandle(args, data, handle);
        }


        /// <summary>
        /// 转格式表格
        /// </summary>
        /// <typeparam name="TData">数据类型</typeparam>
        /// <param name="data">数据</param>
        /// <param name="args">配置</param>
        /// <returns>结果</returns>
        public static string ToFormatStringTable<TData>(this IEnumerable<TData> data,
            WriteStringConfigSpreadSheetArgs args = null)
#if NET_8
            where TData notnul;
#else
            where TData : class
#endif
        {
            return (string)SpreadsheetKit.WriteSpreadSheet(args ?? new WriteStringConfigSpreadSheetArgs(), data);
        }

        /// <summary>
        /// 转CSV
        /// </summary>
        /// <typeparam name="TData">数据类型</typeparam>
        /// <param name="data">数据</param>
        /// <param name="args">配置</param>
        /// <returns>结果</returns>
        public static string ToCsv<TData>(this IEnumerable<TData> data, WriteCsvConfigSpreadSheetArgs args = null)
#if NET_8
            where TData notnul;
#else
            where TData : class
#endif
        {
            return (string)SpreadsheetKit.WriteSpreadSheet(args ?? new WriteCsvConfigSpreadSheetArgs(), data);
        }

        /// <summary>
        /// 转Excel
        /// </summary>
        /// <typeparam name="TData">数据类型</typeparam>
        /// <param name="data">数据</param>
        /// <param name="args">配置</param>
        /// <returns>结果</returns>
        public static byte[] ToExcel<TData>(this IEnumerable<TData> data, WriteExcelConfigSpreadSheetArgs args = null)
#if NET_8
            where TData notnul;
#else
            where TData : class
#endif
        {
            return (byte[])SpreadsheetKit.WriteSpreadSheet(args ?? new WriteExcelConfigSpreadSheetArgs(), data);
        }

        /// <summary>
        /// 转Excel
        /// </summary>
        /// <typeparam name="TData">数据类型</typeparam>
        /// <param name="data">数据</param>
        /// <param name="args">配置</param>
        /// <returns>结果</returns>
        public static EasyModelToExcelWithConfig<TData> ToExcelWithConfig<TData>(this IEnumerable<TData> data, DynamicDataWriteExcelConfigSpreadSheetArgs args = null)
#if NET_8
            where TData notnul;
#else
            where TData : class
#endif
        {
            return new EasyModelToExcelWithConfig<TData>(data, args);
        }

        /// <summary>
        /// 写文本表格
        /// </summary>
        /// <typeparam name="TKey">列定义</typeparam>
        /// <typeparam name="TValue">行定义</typeparam>
        /// <param name="data">数据</param>
        /// <param name="args">配置</param>
        /// <returns>写入结果</returns>
        public static string ToFormatStringTable<TKey, TValue>(this IEnumerable<Dictionary<TKey, TValue>> data,
            WriteStringConfigSpreadSheetArgs args = null)
#if NET_8
            where TKey notnul;
#else
            where TKey : class
#endif
        {
            return (string)SpreadsheetKit.WriteSpreadSheet(args ?? new WriteStringConfigSpreadSheetArgs(), data);
        }

        /// <summary>
        /// 写csv表格
        /// </summary>
        /// <typeparam name="TKey">列定义</typeparam>
        /// <typeparam name="TValue">行定义</typeparam>
        /// <param name="data">数据</param>
        /// <param name="args">配置</param>
        /// <returns>写入结果</returns>
        public static string ToCsv<TKey, TValue>(this IEnumerable<Dictionary<TKey, TValue>> data,
            WriteCsvConfigSpreadSheetArgs args = null)
#if NET_8
            where TKey notnul;
#else
            where TKey : class
#endif
        {
            return (string)SpreadsheetKit.WriteSpreadSheet(args ?? new WriteCsvConfigSpreadSheetArgs(), data);
        }

        /// <summary>
        /// 写excel表格
        /// </summary>
        /// <typeparam name="TKey">列定义</typeparam>
        /// <typeparam name="TValue">行定义</typeparam>
        /// <param name="data">数据</param>
        /// <param name="args">配置</param>
        /// <returns>写入结果</returns>
        public static byte[] ToExcel<TKey, TValue>(this IEnumerable<Dictionary<TKey, TValue>> data,
            DynamicDataWriteExcelConfigSpreadSheetArgs args = null)
#if NET_8
            where TKey notnul;
#else
            where TKey : class
#endif
        {
            return (byte[])SpreadsheetKit.WriteSpreadSheet(args ?? new DynamicDataWriteExcelConfigSpreadSheetArgs(), data);
        }
        /// <summary>
        /// 写excel表格
        /// </summary>
        /// <typeparam name="TKey">列定义</typeparam>
        /// <typeparam name="TValue">行定义</typeparam>
        /// <param name="data">数据</param>
        /// <param name="args">配置</param>
        /// <returns>写入结果</returns>
        public static EasyDicToExcelWithConfig<TKey, TValue> ToExcelWithConfig<TKey, TValue>(this IEnumerable<Dictionary<TKey, TValue>> data,
            DynamicDataWriteExcelConfigSpreadSheetArgs args = null)
#if NET_8
            where TKey notnul;
#else
            where TKey : class
#endif
        {
            return new EasyDicToExcelWithConfig<TKey, TValue>(data, args);
        }


        /// <summary>
        /// 写文本表格
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="args">配置</param>
        /// <returns>写入结果</returns>
        public static string ToFormatStringTable(this DataTable data,
            WriteStringConfigSpreadSheetArgs args = null)
        {
            return (string)SpreadsheetKit.WriteSpreadSheet(args ?? new WriteStringConfigSpreadSheetArgs(), data);
        }

        /// <summary>
        /// 写csv表格
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="args">配置</param>
        /// <returns>写入结果</returns>
        public static string ToCsv(this DataTable data,
            WriteCsvConfigSpreadSheetArgs args = null)
        {
            return (string)SpreadsheetKit.WriteSpreadSheet(args ?? new WriteCsvConfigSpreadSheetArgs(), data);
        }

        /// <summary>
        /// 写excel表格
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="args">配置</param>
        /// <returns>写入结果</returns>
        public static byte[] ToExcel(this DataTable data, DynamicDataWriteExcelConfigSpreadSheetArgs args = null)
        {
            return (byte[])SpreadsheetKit.WriteSpreadSheet(args ?? new DynamicDataWriteExcelConfigSpreadSheetArgs(), data);
        }


        /// <summary>
        /// 写excel表格
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="args">配置</param>
        /// <returns>写入结果</returns>
        public static EasyDataTableToExcelWithConfig ToExcelWithConfig(this DataTable data, DynamicDataWriteExcelConfigSpreadSheetArgs args = null)
        {
            return new EasyDataTableToExcelWithConfig(data,args);
        }


        /// <summary>
        /// 读取数据到动态类型
        /// </summary>
        /// <param name="excelData">数据</param>
        /// <param name="withHead">是否携带头</param>
        /// <returns>动态结果</returns>
        public static List<dynamic> ReadExcelDynamicAutoType(this byte[] excelData, bool withHead = true)
        {
            return SpreadsheetKit.ReadDynamicAutoType(new ReadExcelSpreadSheetArgs(excelData, withHead));
        }

        /// <summary>
        /// 读取数据到动态类型
        /// </summary>
        /// <param name="csvData">数据</param>
        /// <param name="withHead">是否携带头</param>
        /// <returns>动态结果</returns>
        public static List<dynamic> ReadCsvDynamicAutoType(this string csvData, bool withHead = true)
        {
            return SpreadsheetKit.ReadDynamicAutoType(new ReadCsvSpreadSheetArgs(csvData, withHead));
        }

        /// <summary>
        /// 读取数据到动态类型
        /// </summary>
        /// <param name="excelData">数据</param>
        /// <param name="withHead">是否携带头</param>
        /// <returns>动态结果</returns>
        public static List<dynamic> ReadExcelDynamic(this byte[] excelData, bool withHead = true)
        {
            return SpreadsheetKit.ReadDynamic(new ReadExcelSpreadSheetArgs(excelData, withHead));
        }

        /// <summary>
        /// 读取数据到动态类型
        /// </summary>
        /// <param name="csvData">数据</param>
        /// <param name="withHead">是否携带头</param>
        /// <returns>动态结果</returns>
        public static List<dynamic> ReadCsvDynamic(this string csvData, bool withHead = true)
        {
            return SpreadsheetKit.ReadDynamic(new ReadCsvSpreadSheetArgs(csvData, withHead));
        }

        /// <summary>
        /// 读取到字典
        /// </summary>
        /// <param name="excelData">excel数据</param>
        /// <param name="withHead">是否携带头</param>
        /// <returns>动态字典</returns>
        public static List<Dictionary<string, string>> ReadExcelDict(this byte[] excelData, bool withHead = true)
        {
            return SpreadsheetKit.ReadDict(new ReadExcelSpreadSheetArgs(excelData, withHead));
        }

        /// <summary>
        /// 读取到字典
        /// </summary>
        /// <param name="csvData">csv数据</param>
        /// <param name="withHead">是否携带头</param>
        /// <returns>动态字典</returns>
        public static List<Dictionary<string, string>> ReadCsvDict(this string csvData, bool withHead = true)
        {
            return SpreadsheetKit.ReadDict(new ReadCsvSpreadSheetArgs(csvData, withHead));
        }

        /// <summary>
        /// 读取到动态字典
        /// </summary>
        /// <param name="excelData">excel数据</param>
        /// <param name="withHead">是否携带头</param>
        /// <returns>动态字典</returns>
        public static List<Dictionary<string, dynamic>> ReadExcelDictAutoType(this byte[] excelData,
            bool withHead = true)
        {
            return SpreadsheetKit.ReadDictAutoType(new ReadExcelSpreadSheetArgs(excelData, withHead));
        }

        /// <summary>
        /// 读取到动态字典
        /// </summary>
        /// <param name="csvData">数据</param>
        /// <param name="withHead">是否携带头</param>
        /// <returns>动态字典</returns>
        public static List<Dictionary<string, dynamic>> ReadCsvDictAutoType(this string csvData, bool withHead = true)
        {
            return SpreadsheetKit.ReadDictAutoType(new ReadCsvSpreadSheetArgs(csvData, withHead));
        }

        /// <summary>
        /// 读取Excel到强模型
        /// </summary>
        /// <typeparam name="TData">强模型</typeparam>
        /// <param name="excelData">数据</param>
        /// <param name="withHead">是否携带头</param>
        /// <returns>强模型数据</returns>
        public static List<TData> ReadExcelMapModel<TData>(this byte[] excelData, bool withHead = true)
#if NET_8
            where TData notnul
#else
            where TData : class
#endif
        {
            return SpreadsheetKit.ReadModel<TData>(new ReadExcelSpreadSheetArgs(excelData, withHead));
        }


        /// <summary>
        /// 读取CSV到强模型
        /// </summary>
        /// <typeparam name="TData">强模型</typeparam>
        /// <param name="csvData">数据</param>
        /// <param name="withHead">是否携带头</param>
        /// <returns>强模型数据</returns>
        public static List<TData> ReadCsvMapModel<TData>(this string csvData, bool withHead = true)
#if NET_8
            where TData notnul
#else
            where TData : class
#endif
        {
            return SpreadsheetKit.ReadModel<TData>(new ReadCsvSpreadSheetArgs(csvData, withHead));
        }




    }
}