using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Text;
using SegaCyrus.ToolKits.Extension.CommonExtension;
using SegaCyrus.ToolKits.Internal;
using SegaCyrus.ToolKits.Kit;
using SegaCyrus.ToolKits.Model.Common;
using SegaCyrus.ToolKits.Model.Expression;
using SegaCyrus.ToolKits.Model.SpreadSheet.Append;
using SegaCyrus.ToolKits.Model.SpreadSheet.Attributes;
using SegaCyrus.ToolKits.Model.SpreadSheet.Convert;
using SegaCyrus.ToolKits.Model.SpreadSheet.Read;
using SegaCyrus.ToolKits.Model.SpreadSheet.Write;

namespace SegaCyrus.ToolKits.Package.SpreadSheetPackage.Base
{
    /// <summary>
    /// 电子表格操作工具包
    /// </summary>
    public abstract class SpreadSheetPolicy : ISpreadSheetPolicy
    {
        #region AppendData

        /// <summary>
        /// 写数据到电子表格文件
        /// </summary>
        /// <typeparam name="TKey">列定义</typeparam>
        /// <typeparam name="TValue">行定义</typeparam>
        /// <param name="configArgs">数据配置</param>
        /// <param name="data">数据</param>
        /// <param name="handle">追加句柄，若为NULL则新建一个句柄</param>
        /// <returns>写入结果</returns>
        public WriteSpreadSheetHandle WriteSpreadSheetHandle<TKey, TValue>(WriteConfigSpreadSheetArgs configArgs,
            IEnumerable<Dictionary<TKey, TValue>> data, WriteSpreadSheetHandle handle)
        {
            return AppendSpreadSheetData(configArgs, InnerWriteModel(configArgs, data), handle, null);
        }

        /// <summary>
        /// 写数据到电子表格文件
        /// </summary>
        /// <typeparam name="TData">定义</typeparam>
        /// <param name="configArgs">数据配置</param>
        /// <param name="data">数据</param>
        /// <param name="handle">追加句柄，若为NULL则新建一个句柄</param>
        /// <returns>写入结果</returns>
        public WriteSpreadSheetHandle WriteSpreadSheetHandle<TData>(WriteConfigSpreadSheetArgs configArgs,
            IEnumerable<TData> data, WriteSpreadSheetHandle handle)
        {
            return AppendSpreadSheetData(configArgs, InnerWriteModel(configArgs, data), handle, typeof(TData));
        }

        /// <summary>
        /// 写数据到电子表格文件
        /// </summary>
        /// <param name="configArgs">数据配置</param>
        /// <param name="data">数据</param>
        /// <param name="handle">追加句柄，若为NULL则新建一个句柄</param>
        /// <returns>写入结果</returns>
        public WriteSpreadSheetHandle AppendSpreadSheetData(WriteConfigSpreadSheetArgs configArgs, DataTable data,
            WriteSpreadSheetHandle handle)
        {
            return AppendSpreadSheetData(configArgs, InnerWriteModel(configArgs, data), handle, null);
        }

        #endregion

        #region Write

        /// <summary>
        /// 电子表格写入
        /// </summary>
        /// <typeparam name="TKey">Key Type</typeparam>
        /// <typeparam name="TValue">Value Type</typeparam>
        /// <param name="configArgs">配置参数
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
        public virtual object WriteSpreadSheet<TKey, TValue>(WriteConfigSpreadSheetArgs configArgs,
            IEnumerable<Dictionary<TKey, TValue>> data)
        {
            return WriteModel(configArgs, InnerWriteModel(configArgs, data), null);
        }

        /// <summary>
        /// 电子表格写入
        /// </summary>
        /// <typeparam name="TData">Data Type</typeparam>
        /// <param name="configArgs">配置参数
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
        public virtual object WriteSpreadSheet<TData>(WriteConfigSpreadSheetArgs configArgs, IEnumerable<TData> data)
        {
            if (configArgs.UseExpressTree)
                return WriteModel(configArgs, InnerWriteModelExpressTree(configArgs, data), typeof(TData));
            return WriteModel(configArgs, InnerWriteModel(configArgs, data), typeof(TData));
        }

        /// <summary>
        /// 电子表格写入
        /// </summary>
        /// <param name="configArgs">配置参数
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
        public virtual object WriteSpreadSheet(WriteConfigSpreadSheetArgs configArgs, DataTable data)
        {
            return WriteModel(configArgs, InnerWriteModel(configArgs, data), null);
        }

        /// <summary>
        /// 读取数据转二维数组
        /// </summary>
        /// <param name="configArgs">配置</param>
        /// <param name="data">数据</param>
        /// <returns>二位数组</returns>
        protected virtual KVModel<Dictionary<string, int>, List<List<string>>> InnerWriteModel(
            WriteConfigSpreadSheetArgs configArgs, DataTable data)
        {
            var result = new List<List<string>>();
            var headLine = new List<string>();
            foreach (DataColumn item in data.Columns)
                headLine.Add(item.ColumnName);

            if (configArgs.AddHead)
            {
                result.Add(headLine);
            }

            foreach (DataRow row in data.Rows)
            {
                var line = new List<string>();
                foreach (var column in headLine)
                    line.Add(row[column]?.ToString() ?? string.Empty);
                result.Add(line);
            }

            var i = 0;
            var columnMap = headLine.Select(c => new KVModel<string, int>(c, i++))
                .ToDictionary(c => c.Key, c => c.Value);
            return OrderColumn(configArgs, new KVModel<Dictionary<string, int>, List<List<string>>>(columnMap, result));
        }

        /// <summary>
        /// 读取数据转二维数组
        /// </summary>
        /// <param name="configArgs">配置</param>
        /// <param name="data">数据</param>
        /// <returns>二位数组</returns>
        protected KVModel<Dictionary<string, int>, List<List<string>>> InnerWriteModel<TKey, TValue>(
            WriteConfigSpreadSheetArgs configArgs,
            IEnumerable<Dictionary<TKey, TValue>> data)
        {
            AssertKit.AssertNotEmpty(data, nameof(data));
            AssertKit.AssertNotNull(configArgs, nameof(configArgs));
            var result = new List<List<string>>();
            var i = 0;
            var columnNameTKey = data.SelectMany(c => c.Keys).ToHashSet();
            AssertKit.AssertPositive(columnNameTKey.Count, "必须存在列");

            var columnName = columnNameTKey.Select(c => new KVModel<TKey, int>(c, i++))
                .ToDictionary(c => c.Key, c => c.Value);

            if (configArgs.AddHead)
            {
                result.Add(columnName.Select(c => c.Key.ToString()).ToList());
            }

            foreach (var item in data)
            {
                var rowData = columnName.Select(c => string.Empty).ToList();
                foreach (var itemRowName in columnName)
                    rowData[itemRowName.Value] =
                        item.TryGetValue(itemRowName.Key, out var t) ? t?.ToString() : string.Empty;
                result.Add(rowData);
            }

            return OrderColumn(configArgs,
                new KVModel<Dictionary<string, int>, List<List<string>>>(
                    columnName.ToDictionary(c => c.Key?.ToString(), c => c.Value), result));
        }
        
        /// <summary>
        /// 读取数据转二维数组
        /// </summary>
        /// <param name="configArgs">配置</param>
        /// <param name="data">数据</param>
        /// <returns>二位数组</returns>
        protected virtual KVModel<Dictionary<string, int>, List<List<string>>> InnerWriteModel<TData>(
            WriteConfigSpreadSheetArgs configArgs,
            IEnumerable<TData> data)
        {
            var result = new List<List<string>>();
            var typeList = GetWritePropertyList<TData>()
                .Select(c => Tuple.Create(c.Item1, c.Item2,
                    c.Item2.GetCustomAttribute<SpreadSheetConvertAttribute>(true), c.Item3))
                .Select(c => Tuple.Create(c.Item1, c.Item2,
                    c.Item3 == null
                        ? null
                        : (ISpreadSheetDataConvert)ReflectKit.CreateInstance(c.Item3.ConvertImplType), c.Item4))
                .ToList();

            var columnName = typeList.Select(c => c.Item1).ToList();
            if (configArgs.AddHead)
            {
                result.Add(columnName);
            }

            foreach (var item in data)
            {
                var itemRow = new List<string>();
                foreach (var row in typeList)
                {
                    if (row == null)
                        throw new NullReferenceException($"{nameof(data)}存在行数据为null的非法数据");
                    var cellData = row.Item3 == null
                        ? row.Item2.GetValue(item)?.ToString()
                        : row.Item3.Write(row.Item2.GetValue(item));
                    if (row.Item4 == null)
                        itemRow.Add(cellData ?? string.Empty);
                    else
                    {
                        if (cellData == null)
                            itemRow.Add(row.Item4.NullDefaultValue);
                        else if (string.IsNullOrEmpty(cellData))
                            itemRow.Add(row.Item4.DefaultValue);
                        else
                            itemRow.Add(cellData);
                    }
                }

                result.Add(itemRow);
            }

            var i = 0;
            var columnMap = columnName.Select(c => new KVModel<string, int>(c, i++))
                .ToDictionary(c => c.Key, c => c.Value);
            return OrderColumn(configArgs, new KVModel<Dictionary<string, int>, List<List<string>>>(columnMap, result));
        }
        /// <summary>
        /// 读取数据转二维数组
        /// </summary>
        /// <param name="configArgs">配置</param>
        /// <param name="data">数据</param>
        /// <returns>二位数组</returns>
        protected virtual KVModel<Dictionary<string, int>, List<List<string>>> InnerWriteModelExpressTree<TData>(
            WriteConfigSpreadSheetArgs configArgs,
            IEnumerable<TData> data)
        {
            var result = new List<List<string>>();
            var typeList = GetWritePropertyExpressTreeList<TData>()
                .Select(c => Tuple.Create(
                    c.Item1,
                    c.Item3,
                    c.Item2.GetCustomAttribute<SpreadSheetConvertAttribute>(true)))
                .Select(c => Tuple.Create(
                    c.Item1,
                    c.Item2,
                    c.Item3 == null ? null : (ISpreadSheetDataConvert)ReflectKit.CreateInstance(c.Item3.ConvertImplType)))
                .ToList();

            var columnName = typeList.Select(c => c.Item1).ToList();
            if (configArgs.AddHead)
            {
                result.Add(columnName);
            }

            foreach (var item in data)
            {
                var itemRow = new List<string>();
                foreach (var row in typeList)
                {
                    if (row == null)
                        throw new NullReferenceException($"{nameof(data)}存在行数据为null的非法数据");
                    var cellData = row.Item3 == null
                        ? row.Item2.Get(item)?.ToString()
                        : row.Item3.Write(row.Item2.Get(item));
                    itemRow.Add(cellData ?? string.Empty);
                }

                result.Add(itemRow);
            }

            var i = 0;
            var columnMap = columnName.Select(c => new KVModel<string, int>(c, i++))
                .ToDictionary(c => c.Key, c => c.Value);
            return OrderColumn(configArgs, new KVModel<Dictionary<string, int>, List<List<string>>>(columnMap, result));
        }

        #endregion

        #region Read

        /// <summary>
        /// 电子表格读取
        /// </summary>
        /// <param name="args">
        /// 配置参数
        /// <seealso cref="WriteCsvConfigSpreadSheetArgs"/>
        /// <seealso cref="WriteExcelConfigSpreadSheetArgs"/>
        /// <seealso cref="WriteStringConfigSpreadSheetArgs"/>
        /// </param>
        /// <returns>返回对应的强模型</returns>
        public virtual List<T> ReadModel<T>(ReadSpreadSheetArgs args)
        {
            var result = ReadData(args);
            if(args.UseExpressTree)
                return ConvertToModelExpressTree<T>(result, args.HasHead);
            return ConvertToModel<T>(result, args.HasHead);
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
        public virtual List<dynamic> ReadDynamicAutType(ReadSpreadSheetArgs args)
        {
            var result = ReadData(args);
            return ConvertToDynamicAutoType(result, args.HasHead);
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
        public virtual List<Dictionary<string, dynamic>> ReadDictAutoType(ReadSpreadSheetArgs args)
        {
            var result = ReadData(args);
            return ConvertToDicAutoType(result, args.HasHead);
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
        /// <returns>返回对应的动态类型, 对应属性为String</returns>
        public virtual List<Dictionary<string, string>> ReadDict(ReadSpreadSheetArgs args)
        {
            var result = ReadData(args);
            return ConvertToDic(result, args.HasHead);
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
        /// <returns>返回对应的动态类型, 对应属性为String</returns>
        public virtual List<dynamic> ReadDynamic(ReadSpreadSheetArgs args)
        {
            var result = ReadData(args);
            return ConvertToDynamic(result, args.HasHead);
        }

        #endregion

        /// <summary>
        /// 通用写读
        /// </summary>
        /// <param name="configArgs">参数配置</param>
        /// <param name="data">数据</param>
        /// <param name="handle">写入句柄</param>
        /// <param name="type">类型</param>
        /// <returns>返回转换结果</returns>
        protected abstract WriteSpreadSheetHandle AppendSpreadSheetData(WriteConfigSpreadSheetArgs configArgs,
            KVModel<Dictionary<string, int>, List<List<string>>> data, WriteSpreadSheetHandle handle, Type type = null);

        /// <summary>
        /// 通用写读
        /// </summary>
        /// <param name="configArgs">参数配置</param>
        /// <param name="data">数据</param>
        /// <param name="type">类型</param>
        /// <returns>返回转换结果</returns>
        protected abstract object WriteModel(WriteConfigSpreadSheetArgs configArgs,
            KVModel<Dictionary<string, int>, List<List<string>>> data, Type type = null);

        /// <summary>
        /// 通用读
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        protected abstract List<List<string>> ReadData(ReadSpreadSheetArgs args);

        /// <summary>
        /// 通用顺序调整
        /// </summary>
        /// <param name="config">配置</param>
        /// <param name="data">数据</param>
        /// <returns></returns>
        protected KVModel<Dictionary<string, int>, List<List<string>>> OrderColumn(WriteConfigSpreadSheetArgs config,
            KVModel<Dictionary<string, int>, List<List<string>>> data)
        {
            var column = config.ColumnOrder(data.Key.Keys.ToList());
            if (column == null || data.Value.IsEmpty())
                return data;

            var currentColumnOrder = data.Key.Select(c => new KVModel<string, int>(c.Key, c.Value))
                .OrderBy(c => c.Value).ToList();
            if (column.Count != currentColumnOrder.Count)
                goto ReOrder;

            for (var i = 0; i < column.Count && i < currentColumnOrder.Count; ++i)
            {
                if (column[i] != currentColumnOrder[i].Key)
                    goto ReOrder;
            }

            return data;
            ReOrder:
            var newPol = new Dictionary<string, int>();
            var result = new List<List<string>>();
            var transposeData = MathExtension.Transpose(data.Value);

            var j = 0;
            foreach (var item in column)
            {
                newPol[item] = j++;
                if (data.Key.TryGetValue(item, out var key))
                {
                    result.Add(transposeData[key]);
                    transposeData[key] = null;
                }
                else
                {
                    var emptyLine = transposeData.First().Select(c => string.Empty).ToList();
                    if (config.AddHead)
                        emptyLine[0] = item;
                    result.Add(emptyLine);
                }
            }

            if (transposeData.Exists(c => c != null))
            {
                var dataMap = currentColumnOrder
                    .Where(c => !newPol.ContainsKey(c.Key))
                    .OrderBy(c => c.Value)
                    .Select(c => new KVModel<string, int>(c.Key, j++));

                foreach (var item in dataMap)
                    newPol[item.Key] = item.Value;

                result.AddRange(transposeData.Where(c => c != null));
            }

            return new KVModel<Dictionary<string, int>, List<List<string>>>(newPol, MathExtension.Transpose(result));
        }

        /// <summary>
        /// SpreadsheetDataSet模型转动态模型
        /// </summary>
        /// <param name="data"></param>
        /// <param name="hasHead"></param>
        /// <returns></returns>
        protected List<Dictionary<string, string>> ConvertToDic(List<List<string>> data, bool hasHead)
        {
            var result = new List<Dictionary<string, string>>();
            List<string> headLine;
            List<List<string>> convertResult;
            var withHead = hasHead;
            var dataSet = data;

            ReDo:
            if (withHead)
            {
                var convertResTmp = dataSet;
                headLine = convertResTmp.First();
                convertResTmp.RemoveAt(0);
                for (var i = 0; i < convertResTmp.Count; ++i)
                {
                    var item = convertResTmp.ElementAt(i);
                    if (item.Count == headLine.Count)
                        continue;
                    if (item.Count > headLine.Count)
                        convertResTmp[i] = item.Take(headLine.Count).ToList();
                    else if (item.Count < headLine.Count)
                    {
                        var count = headLine.Count - item.Count;
                        for (var j = 0; j < count; ++j)
                            item.Add(string.Empty);
                    }
                }

                convertResult = convertResTmp;
            }
            else
            {
                var maxColumn = data.Select(c => c.Count).Max();
                var headLineTmp = BuildDefaultColumnName(maxColumn);
                var tmp = dataSet.ToList();
                tmp.Insert(0, headLineTmp);
                dataSet = tmp;
                withHead = true;
                goto ReDo;
            }

            for (var i = 0; i < convertResult.Count; ++i)
                result.Add(new Dictionary<string, string>());

            convertResult = MathExtension.Transpose(convertResult);

            for (var i = 0; i < headLine.Count; ++i)
            {
                var typeList = convertResult[i].Select(c => ReflectKit.TryGetType(c))
                    .Where(c => c != null)
                    .ToHashSet();
                for (var j = 0; j < convertResult[i].Count; ++j)
                {
                    var x = result[j] as IDictionary<string, string>;
                    x[headLine[i]] = convertResult[i][j];
                }
            }

            return result;
        }

        /// <summary>
        /// SpreadsheetDataSet模型转动态模型
        /// </summary>
        /// <param name="data"></param>
        /// <param name="hasHead"></param>
        /// <returns></returns>
        protected List<Dictionary<string, dynamic>> ConvertToDicAutoType(List<List<string>> data, bool hasHead)
        {
            var result = new List<Dictionary<string, dynamic>>();
            List<string> headLine;
            List<List<string>> convertResult;
            var withHead = hasHead;
            var dataSet = data;

            ReDo:
            if (withHead)
            {
                var convertResTmp = dataSet;
                headLine = convertResTmp.First();
                convertResTmp.RemoveAt(0);
                for (var i = 0; i < convertResTmp.Count; ++i)
                {
                    var item = convertResTmp.ElementAt(i);
                    if (item.Count == headLine.Count)
                        continue;
                    if (item.Count > headLine.Count)
                        convertResTmp[i] = item.Take(headLine.Count).ToList();
                    else if (item.Count < headLine.Count)
                    {
                        var count = headLine.Count - item.Count;
                        for (var j = 0; j < count; ++j)
                            item.Add(string.Empty);
                    }
                }

                convertResult = convertResTmp;
            }
            else
            {
                var maxColumn = data.Select(c => c.Count).Max();
                var headLineTmp = BuildDefaultColumnName(maxColumn);
                var tmp = dataSet.ToList();
                tmp.Insert(0, headLineTmp);
                dataSet = tmp;
                withHead = true;
                goto ReDo;
            }

            for (var i = 0; i < convertResult.Count; ++i)
                result.Add(new Dictionary<string, dynamic>());

            convertResult = MathExtension.Transpose(convertResult);

            for (var i = 0; i < headLine.Count; ++i)
            {
                var typeList = convertResult[i].Select(c => ReflectKit.TryGetType(c))
                    .Where(c => c != null)
                    .ToHashSet();
                var convertFun = typeList.NotEmpty()
                    ? ReflectKit.GetConvertDataHandle(typeList.First())
                    : ReflectKit.GetConvertDataHandle(typeof(string));
                for (var j = 0; j < convertResult[i].Count; ++j)
                {
                    var x = result[j] as IDictionary<string, dynamic>;
                    x[headLine[i]] = convertFun(convertResult[i][j]);
                }
            }

            return result;
        }

        /// <summary>
        /// SpreadsheetDataSet模型转动态模型
        /// </summary>
        /// <param name="data"></param>
        /// <param name="hasHead"></param>
        /// <returns></returns>
        protected List<dynamic> ConvertToDynamicAutoType(List<List<string>> data, bool hasHead)
        {
            var result = new List<ExpandoObject>();
            List<string> headLine;
            List<List<string>> convertResult;
            var withHead = hasHead;
            var dataSet = data;

            ReDo:
            if (withHead)
            {
                var convertResTmp = dataSet;
                headLine = convertResTmp.First();
                convertResTmp.RemoveAt(0);
                for (var i = 0; i < convertResTmp.Count; ++i)
                {
                    var item = convertResTmp.ElementAt(i);
                    if (item.Count == headLine.Count)
                        continue;
                    if (item.Count > headLine.Count)
                        convertResTmp[i] = item.Take(headLine.Count).ToList();
                    else if (item.Count < headLine.Count)
                    {
                        var count = headLine.Count - item.Count;
                        for (var j = 0; j < count; ++j)
                            item.Add(string.Empty);
                    }
                }

                convertResult = convertResTmp;
            }
            else
            {
                var maxColumn = data.Select(c => c.Count).Max();
                var headLineTmp = BuildDefaultColumnName(maxColumn);
                var tmp = dataSet.ToList();
                tmp.Insert(0, headLineTmp);
                dataSet = tmp;
                withHead = true;
                goto ReDo;
            }

            for (var i = 0; i < convertResult.Count; ++i)
                result.Add(new ExpandoObject());

            convertResult = MathExtension.Transpose(convertResult);

            for (var i = 0; i < headLine.Count; ++i)
            {
                var typeList = convertResult[i].Select(c => ReflectKit.TryGetType(c))
                    .Where(c => c != null)
                    .ToHashSet();
                var convertFun = typeList.NotEmpty()
                    ? ReflectKit.GetConvertDataHandle(typeList.First())
                    : ReflectKit.GetConvertDataHandle(typeof(string));
                for (var j = 0; j < convertResult[i].Count; ++j)
                {
                    var x = result[j] as IDictionary<string, object>;
                    x[headLine[i]] = convertFun(convertResult[i][j]);
                }
            }

            var res = new List<dynamic>();
            foreach (var item in result)
                res.Add(item);

            return res;
        }


        /// <summary>
        /// SpreadsheetDataSet模型转动态模型
        /// </summary>
        /// <param name="data"></param>
        /// <param name="hasHead"></param>
        /// <returns></returns>
        protected List<dynamic> ConvertToDynamic(List<List<string>> data, bool hasHead)
        {
            var result = new List<ExpandoObject>();
            List<string> headLine;
            List<List<string>> convertResult;
            var withHead = hasHead;
            var dataSet = data;

            ReDo:
            if (withHead)
            {
                var convertResTmp = dataSet;
                headLine = convertResTmp.First();
                convertResTmp.RemoveAt(0);
                for (var i = 0; i < convertResTmp.Count; ++i)
                {
                    var item = convertResTmp.ElementAt(i);
                    if (item.Count == headLine.Count)
                        continue;
                    if (item.Count > headLine.Count)
                        convertResTmp[i] = item.Take(headLine.Count).ToList();
                    else if (item.Count < headLine.Count)
                    {
                        var count = headLine.Count - item.Count;
                        for (var j = 0; j < count; ++j)
                            item.Add(string.Empty);
                    }
                }

                convertResult = convertResTmp;
            }
            else
            {
                var maxColumn = data.Select(c => c.Count).Max();
                var headLineTmp = BuildDefaultColumnName(maxColumn);
                var tmp = dataSet.ToList();
                tmp.Insert(0, headLineTmp);
                dataSet = tmp;
                withHead = true;
                goto ReDo;
            }

            for (var i = 0; i < convertResult.Count; ++i)
                result.Add(new ExpandoObject());

            convertResult = MathExtension.Transpose(convertResult);

            for (var i = 0; i < headLine.Count; ++i)
            {
                var typeList = convertResult[i].Select(ReflectKit.TryGetType)
                    .Where(c => c != null)
                    .ToHashSet();
                for (var j = 0; j < convertResult[i].Count; ++j)
                {
                    var x = result[j] as IDictionary<string, object>;
                    x[headLine[i]] = convertResult[i][j];
                }
            }

            var res = new List<dynamic>();
            foreach (var item in result)
                res.Add(item);

            return res;
        }

        /// <summary>
        /// 转强模型
        /// </summary>
        /// <typeparam name="T">目标模型</typeparam>
        /// <param name="spreadSheetData"></param>
        /// <param name="hasHead"></param>
        /// <returns></returns>
        protected List<T> ConvertToModel<T>(List<List<string>> spreadSheetData, bool hasHead)
        {
            var result = new List<T>();
            var readProperty = GetReadPropertyList<T>()
                .Select(c => Tuple.Create(c.Item1, c.Item2,
                    c.Item3, c.Item3.GetCustomAttribute<SpreadSheetConvertAttribute>(true)))
                .Select(c => Tuple.Create(c.Item1, c.Item2, c.Item3,
                    c.Item4 == null
                        ? null
                        : (ISpreadSheetDataConvert)ReflectKit.CreateInstance(c.Item4.ConvertImplType)))
                .ToList();

            if (readProperty.IsEmpty() || spreadSheetData.IsEmpty())
                return result;

            var internalSpreadSheetData = spreadSheetData.ToList();

            if (hasHead == false)
                readProperty = readProperty.Where(c => c.Item2.HasValue).ToList();
            else if (internalSpreadSheetData.Count == 1 && hasHead)
                return result;

            var typeDefine = typeof(T);

            var mapData = new Dictionary<int, Tuple<PropertyInfo, Func<string, object>>>();

            if (hasHead)
            {
                var i = 0;
                var map = new Dictionary<string, int>();
                var data = internalSpreadSheetData.First();
                foreach (var item in data)
                    map[item] = i++;

                foreach (var item in readProperty)
                {
                    var convertHandle = item.Item4 != null
                        ? item.Item4.Read
                        : ReflectKit.GetConvertDataHandle(item.Item3.PropertyType);
                    if (map.TryGetValue(item.Item1, out var p1))
                        mapData[p1] = Tuple.Create(item.Item3, convertHandle);
                    else if (item.Item2.HasValue)
                        mapData[item.Item2.Value] =
                            Tuple.Create(item.Item3, convertHandle);
                }

                internalSpreadSheetData.RemoveAt(0);
            }
            else
            {
                foreach (var item in readProperty)
                    mapData[item.Item2.Value] =
                        Tuple.Create(item.Item3,
                            ReflectKit.GetConvertDataHandle(item.Item3.PropertyType));
            }

            foreach (var item in internalSpreadSheetData)
            {
                var instance = (T)ReflectKit.CreateInstance(typeDefine);
                foreach (var proItem in mapData)
                    if (proItem.Key < item.Count)
                        proItem.Value.Item1.SetValue(instance, proItem.Value.Item2(item[proItem.Key]));
                result.Add(instance);
            }

            return result;
        }

        /// <summary>
        /// 转强模型(表达式树实现)
        /// </summary>
        /// <typeparam name="T">目标模型</typeparam>
        /// <param name="spreadSheetData"></param>
        /// <param name="hasHead"></param>
        /// <returns></returns>
        protected List<T> ConvertToModelExpressTree<T>(List<List<string>> spreadSheetData, bool hasHead)
        {
            var result = new List<T>();
            var readProperty = GetReadPropertyListExpressTree<T>()
                .Select(c => Tuple.Create(c.Item1, c.Item2,
                    c.Item3, c.Item3.GetCustomAttribute<SpreadSheetConvertAttribute>(true),c.Item4))
                .Select(c => Tuple.Create(c.Item1, c.Item2, c.Item3,
                    c.Item4 == null
                        ? null
                        : (ISpreadSheetDataConvert)ReflectKit.CreateInstance(c.Item4.ConvertImplType),c.Item5))
                .ToList();

            if (readProperty.IsEmpty() || spreadSheetData.IsEmpty())
                return result;

            var internalSpreadSheetData = spreadSheetData.ToList();

            if (hasHead == false)
                readProperty = readProperty.Where(c => c.Item2.HasValue).ToList();
            else if (internalSpreadSheetData.Count == 1 && hasHead)
                return result;

            var typeDefine = typeof(T);

            var mapData = new Dictionary<int, Tuple<ExpressionGetSetModel<T, object>, Func<string, object>>>();

            if (hasHead)
            {
                var i = 0;
                var map = new Dictionary<string, int>();
                var data = internalSpreadSheetData.First();
                foreach (var item in data)
                    map[item] = i++;

                foreach (var item in readProperty)
                {
                    var convertHandle = item.Item4 != null
                        ? item.Item4.Read
                        : ReflectKit.GetConvertDataHandle(item.Item3.PropertyType);
                    if (map.TryGetValue(item.Item1, out var p1))
                        mapData[p1] = Tuple.Create(item.Item5, convertHandle);
                    else if (item.Item2.HasValue)
                        mapData[item.Item2.Value] =
                           Tuple.Create(item.Item5, convertHandle);
                }

                internalSpreadSheetData.RemoveAt(0);
            }
            else
            {
                foreach (var item in readProperty)
                    mapData[item.Item2.Value] =
                        Tuple.Create(item.Item5,
                            ReflectKit.GetConvertDataHandle(item.Item3.PropertyType));
            }

            foreach (var item in internalSpreadSheetData)
            {
                var instance = (T)ReflectKit.CreateInstance(typeDefine);
                foreach (var proItem in mapData)
                    if (proItem.Key < item.Count)
                        proItem.Value.Item1.Set(instance, proItem.Value.Item2(item[proItem.Key]));
                result.Add(instance);
            }

            return result;
        }

        /// <summary>
        /// 获取所有需要读的属性(表达式树实现)
        /// </summary>
        /// <typeparam name="T">类</typeparam>
        /// <returns>读取列表</returns>
        protected List<Tuple<string, int?, PropertyInfo, ExpressionGetSetModel<T, object>>> GetReadPropertyListExpressTree<T>()
        {
            var type = typeof(T);
            return type.GetProperties()
                .Where(c => c.GetCustomAttribute<SpreadSheetColumnIgnoreAttribute>(true) == null)
                .Select(c =>
                    new KeyValuePair<SpreadSheetColumnDefineAttribute, PropertyInfo>(
                        c.GetCustomAttribute<SpreadSheetColumnDefineAttribute>(true), c))
                .Select(c =>
                    Tuple.Create(c.Key?.PropertyName ?? c.Value.Name, c.Key?.Order, c.Value, ExpressionKit.CreatePropertyExpress<T, object>(type, c.Value.Name)))
                .ToList();
        }

        /// <summary>
        /// 获取所有需要读的属性
        /// </summary>
        /// <typeparam name="T">类</typeparam>
        /// <returns>读取列表</returns>
        protected List<Tuple<string, int?, PropertyInfo>> GetReadPropertyList<T>()
        {
            return typeof(T).GetProperties()
                .Where(c => c.GetCustomAttribute<SpreadSheetColumnIgnoreAttribute>(true) == null)
                .Select(c =>
                    new KeyValuePair<SpreadSheetColumnDefineAttribute, PropertyInfo>(
                        c.GetCustomAttribute<SpreadSheetColumnDefineAttribute>(true), c))
                .Select(c =>
                    Tuple.Create(c.Key?.PropertyName ?? c.Value.Name, c.Key?.Order, c.Value))
                .ToList();
        }

        /// <summary>
        /// 获取强模型中需要操作的属性,并按顺序要求排序命名
        /// </summary>
        /// <param name="type">类定义</param>
        /// <returns>属性集合</returns>
        protected virtual List<Tuple<string, PropertyInfo, SpreadSheetColumnDefineAttribute>> GetWritePropertyList(Type type)
        {
            var tmp = type.GetProperties()
                .Where(c => c.GetCustomAttribute<SpreadSheetColumnIgnoreAttribute>(true) == null)
                .Select(c => Tuple.Create(c, c.GetCustomAttribute<SpreadSheetColumnDefineAttribute>(true))).ToList();
            var order = tmp.Max(c => c.Item2?.Order ?? 0) + 1;
            return tmp.Select(c => Tuple.Create(c.Item2?.Order ?? order++, c.Item1, c.Item2))
                .OrderBy(c => c.Item1)
                .Select(c => Tuple.Create(c.Item3?.PropertyName ?? c.Item2.Name, c.Item2, c.Item3))
                .ToList();
        }

        /// <summary>
        /// 获取强模型中需要操作的属性,并按顺序要求排序命名
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        protected virtual List<Tuple<string, PropertyInfo, SpreadSheetColumnDefineAttribute>> GetWritePropertyList<T>()
        {
            return GetWritePropertyList(typeof(T));
        }
        /// <summary>
        /// 获取强模型中需要操作的属性,并按顺序要求排序命名
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        protected virtual List<Tuple<string, PropertyInfo, ExpressionGetSetModel<T, object>>> GetWritePropertyExpressTreeList<T>()
        {
            var order = 0;
            var type = typeof(T);
            return type.GetProperties()
                .Where(c => c.GetCustomAttribute<SpreadSheetColumnIgnoreAttribute>(true) == null)
                .Select(c =>
                    new KVModel<int, PropertyInfo>(
                        c.GetCustomAttribute<SpreadSheetColumnDefineAttribute>(true)?.Order ?? order++, c))
                .OrderBy(c => c.Key)
                .Select(c =>
                    Tuple.Create(
                        c.Value.GetCustomAttribute<SpreadSheetColumnDefineAttribute>(true)?.PropertyName ??
                        c.Value.Name, c.Value, ExpressionKit.CreatePropertyExpress<T, object>(type, c.Value.Name)))
                .ToList();
        }

        /// <summary>
        /// 构造默认列名,规则为A、B、C···X、Y、Z、AA、AB、AC....
        /// </summary>
        /// <param name="count">构造数量</param>
        /// <returns></returns>
        protected virtual List<string> BuildDefaultColumnName(int count)
        {
            var result = new List<string>();
            for (var i = 1; i <= count; ++i)
            {
                var n = i;
                var s = new StringBuilder();
                while (n > 0)
                {
                    n--;
                    n = Math.DivRem(n, 26, out var rem);
                    s.Append((char)(rem + 'A'));
                }

                result.Add(string.Concat(s.ToString().Reverse()));
            }

            return result;
        }
    }
}