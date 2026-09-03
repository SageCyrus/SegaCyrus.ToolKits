using System.Collections.Generic;
using System.Data;
using SegaCyrus.ToolKits.Model.SpreadSheet.Append;
using SegaCyrus.ToolKits.Model.SpreadSheet.Read;
using SegaCyrus.ToolKits.Model.SpreadSheet.Write;

namespace SegaCyrus.ToolKits.Package.SpreadSheetPackage.Base
{
    /// <summary>
    /// 电子表格基本接口
    /// </summary>
    public interface ISpreadSheetPolicy
    {
        #region 读取

        /// <summary>
        /// 读取电子表格到强模型
        /// </summary>
        /// <typeparam name="T">强模型</typeparam>
        /// <param name="args">数据配置</param>
        /// <returns>读取结果</returns>
        List<T> ReadModel<T>(ReadSpreadSheetArgs args);

        /// <summary>
        /// 读取电子表格到动态对象
        /// </summary>
        /// <param name="args">数据配置</param>
        /// <returns>读取结果</returns>
        List<dynamic> ReadDynamic(ReadSpreadSheetArgs args);

        /// <summary>
        /// 读取电子表格到动态对象
        /// </summary>
        /// <param name="args">数据配置</param>
        /// <returns>读取结果</returns>
        List<dynamic> ReadDynamicAutType(ReadSpreadSheetArgs args);

        /// <summary>
        /// 读取电子表格到动态对象
        /// </summary>
        /// <param name="args">数据配置</param>
        /// <returns>读取结果</returns>
        List<Dictionary<string, string>> ReadDict(ReadSpreadSheetArgs args);

        /// <summary>
        /// 读取电子表格到动态对象
        /// </summary>
        /// <param name="args">数据配置</param>
        /// <returns>读取结果</returns>
        List<Dictionary<string, dynamic>> ReadDictAutoType(ReadSpreadSheetArgs args);

        //List<DataSet> ReadDataSet(ReadSpreadSheetArgs args);

        #endregion

        #region 写入

        /// <summary>
        /// 写数据到电子表格文件
        /// </summary>
        /// <typeparam name="TKey">列定义</typeparam>
        /// <typeparam name="TValue">行定义</typeparam>
        /// <param name="configArgs">数据配置</param>
        /// <param name="data">数据</param>
        /// <returns>写入结果</returns>
        object WriteSpreadSheet<TKey, TValue>(WriteConfigSpreadSheetArgs configArgs,
            IEnumerable<Dictionary<TKey, TValue>> data); //where TKey : notnull;

        /// <summary>
        /// 写数据到电子表格文件
        /// </summary>
        /// <typeparam name="TData"></typeparam>
        /// <param name="configArgs"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        object WriteSpreadSheet<TData>(WriteConfigSpreadSheetArgs configArgs,
            IEnumerable<TData> data); // where TData : notnull;

        /// <summary>
        /// 写数据到电子表格文件
        /// </summary>
        /// <param name="configArgs">数据配置</param>
        /// <param name="data">数据</param>
        /// <returns>写入结果</returns>
        object WriteSpreadSheet(WriteConfigSpreadSheetArgs configArgs, DataTable data);

        #endregion

        #region 追加

        /// <summary>
        /// 写数据到电子表格文件
        /// </summary>
        /// <typeparam name="TKey">列定义</typeparam>
        /// <typeparam name="TValue">行定义</typeparam>
        /// <param name="configArgs">数据配置</param>
        /// <param name="data">数据</param>
        /// <param name="handle">追加句柄，若为NULL则新建一个句柄</param>
        /// <returns>写入结果</returns>
        WriteSpreadSheetHandle WriteSpreadSheetHandle<TKey, TValue>(WriteConfigSpreadSheetArgs configArgs,
            IEnumerable<Dictionary<TKey, TValue>> data, WriteSpreadSheetHandle handle); //where TKey : notnull;

        /// <summary>
        /// 写数据到电子表格文件
        /// </summary>
        /// <typeparam name="TData"></typeparam>
        /// <param name="configArgs"></param>
        /// <param name="data"></param>
        /// <param name="handle">追加句柄，若为NULL则新建一个句柄</param>
        /// <returns></returns>
        WriteSpreadSheetHandle WriteSpreadSheetHandle<TData>(WriteConfigSpreadSheetArgs configArgs,
            IEnumerable<TData> data, WriteSpreadSheetHandle handle); // where TData : notnull;

        /// <summary>
        /// 写数据到电子表格文件
        /// </summary>
        /// <param name="configArgs">数据配置</param>
        /// <param name="data">数据</param>
        /// <param name="handle">追加句柄，若为NULL则新建一个句柄</param>
        /// <returns>写入结果</returns>
        WriteSpreadSheetHandle AppendSpreadSheetData(WriteConfigSpreadSheetArgs configArgs, DataTable data,
            WriteSpreadSheetHandle handle);

        #endregion
    }
}