//using System;
//using System.Collections.Generic;
//using System.Data;
//using System.Linq;
//using SegaCyrus.ToolKits.Extension.CommonExtension;
//using SegaCyrus.ToolKits.Kit;
//using SegaCyrus.ToolKits.Model.Common;
//using SegaCyrus.ToolKits.Model.SpreadSheet.Append;
//using SegaCyrus.ToolKits.Model.SpreadSheet.Attributes;
//using SegaCyrus.ToolKits.Model.SpreadSheet.Enums;
//using SegaCyrus.ToolKits.Model.SpreadSheet.Read;
//using SegaCyrus.ToolKits.Model.SpreadSheet.Write;
//using SegaCyrus.ToolKits.Package.SpreadSheetPackage.Base;
//using SegaCyrus.ToolKits.Package.SQLPackage;

//namespace SegaCyrus.ToolKits.Package.SpreadSheetPackage.Policy
//{
//    [SpreadSheetRegister(SpreadSheetTypeEnum.SQL)]
//    internal class SqlSpreadSheetPolicy : SpreadSheetPolicy
//    {
//        #region Write

//        /// <summary>
//        /// 电子表格写入
//        /// </summary>
//        /// <typeparam name="TKey">Key Type</typeparam>
//        /// <typeparam name="TValue">Value Type</typeparam>
//        /// <param name="configArgs">配置参数
//        /// <seealso cref="WriteCsvConfigSpreadSheetArgs"/>
//        /// <seealso cref="WriteExcelConfigSpreadSheetArgs"/>
//        /// <seealso cref="WriteStringConfigSpreadSheetArgs"/>
//        /// </param>
//        /// <param name="data">转换数据</param>
//        /// <returns>转换结果</returns>
//        /// <remarks>
//        /// 根据参数<paramref name="configArgs"/>决定返回值
//        /// 当参数为<seealso cref="WriteCsvConfigSpreadSheetArgs"/>时 返回值类型为string
//        /// 当参数为<seealso cref="WriteStringConfigSpreadSheetArgs"/>时 返回值类型为string
//        /// 当参数为<seealso cref="WriteExcelConfigSpreadSheetArgs"/>时 返回值类型为bytes[]
//        /// </remarks>
//        public override object WriteSpreadSheet<TKey, TValue>(WriteConfigSpreadSheetArgs configArgs,
//            IEnumerable<Dictionary<TKey, TValue>> data)
//        {
//            return SQLWrite(configArgs, SqlInnerWriteModel(configArgs, data));
//        }

//        /// <summary>
//        /// 电子表格写入
//        /// </summary>
//        /// <typeparam name="TData">Data Type</typeparam>
//        /// <param name="configArgs">配置参数
//        /// <seealso cref="WriteCsvConfigSpreadSheetArgs"/>
//        /// <seealso cref="WriteExcelConfigSpreadSheetArgs"/>
//        /// <seealso cref="WriteStringConfigSpreadSheetArgs"/>
//        /// </param>
//        /// <param name="data">转换数据</param>
//        /// <returns>转换结果</returns>
//        /// <remarks>
//        /// 根据参数<paramref name="configArgs"/>决定返回值
//        /// 当参数为<seealso cref="WriteCsvConfigSpreadSheetArgs"/>时 返回值类型为string
//        /// 当参数为<seealso cref="WriteStringConfigSpreadSheetArgs"/>时 返回值类型为string
//        /// 当参数为<seealso cref="WriteExcelConfigSpreadSheetArgs"/>时 返回值类型为bytes[]
//        /// </remarks>
//        public override object WriteSpreadSheet<TData>(WriteConfigSpreadSheetArgs configArgs, IEnumerable<TData> data)
//        {
//            return SQLWrite(configArgs, SqlInnerWriteModel(configArgs, data));
//        }

//        /// <summary>
//        /// 电子表格写入
//        /// </summary>
//        /// <param name="configArgs">配置参数
//        /// <seealso cref="WriteCsvConfigSpreadSheetArgs"/>
//        /// <seealso cref="WriteExcelConfigSpreadSheetArgs"/>
//        /// <seealso cref="WriteStringConfigSpreadSheetArgs"/>
//        /// </param>
//        /// <param name="data">转换数据</param>
//        /// <returns>转换结果</returns>
//        /// <remarks>
//        /// 根据参数<paramref name="configArgs"/>决定返回值
//        /// 当参数为<seealso cref="WriteCsvConfigSpreadSheetArgs"/>时 返回值类型为string
//        /// 当参数为<seealso cref="WriteStringConfigSpreadSheetArgs"/>时 返回值类型为string
//        /// 当参数为<seealso cref="WriteExcelConfigSpreadSheetArgs"/>时 返回值类型为bytes[]
//        /// </remarks>
//        public override object WriteSpreadSheet(WriteConfigSpreadSheetArgs configArgs, DataTable data)
//        {
//            return SQLWrite(configArgs, data);
//        }

//        #endregion

//        #region SQLInnerWrite

//        private DataTable SqlInnerWriteModel<TKey, TValue>(WriteConfigSpreadSheetArgs configArgs,
//            IEnumerable<Dictionary<TKey, TValue>> data)
//        {
//            AssertKit.AssertNotEmpty(data, nameof(data));
//            AssertKit.AssertNotNull(configArgs, nameof(configArgs));
//            AssertKit.AssertTrue(
//                configArgs.TypeEnum == (int)SpreadSheetTypeEnum.SQL && configArgs is WriteSqlConfigSpreadSheetArgs,
//                msg: "sql必须传入ReadSQLSpreadsheetArgs");
//            var config = configArgs as WriteSqlConfigSpreadSheetArgs;
//            var dataTable = new DataTable();
//            var pool = new Dictionary<string, bool>();
//            var policy = SQLProxy.Instance.GetPolicy(config.SQLType);
//            var columnNameTKey = data.SelectMany(c => c.Keys).ToHashSet();
//            AssertKit.AssertPositive(columnNameTKey.Count, "必须存在列");

//            foreach (var item in data)
//            {
//                var row = dataTable.NewRow();
//                foreach (var rowCell in item)
//                {
//                    var key = rowCell.Key.ToString();
//                    if (rowCell.Value == null || string.IsNullOrEmpty(key))
//                        continue;
//                    if (!pool.ContainsKey(key))
//                    {
//                        var isSqlType = policy.IsSQLType(rowCell.GetType());
//                        dataTable.Columns.Add(new DataColumn(key, isSqlType ? rowCell.GetType() : typeof(string)));
//                        pool[key] = isSqlType;
//                    }

//                    row[key] = pool[key] ? (object)rowCell.Value : rowCell.Value.ToString();
//                }

//                dataTable.Rows.Add(row);
//            }

//            dataTable.TableName = config.TableName;
//            return dataTable;
//        }

//        private DataTable SqlInnerWriteModel<TData>(WriteConfigSpreadSheetArgs configArgs, IEnumerable<TData> data)
//        {
//            AssertKit.AssertNotNull(configArgs, nameof(configArgs));
//            AssertKit.AssertTrue(
//                configArgs.TypeEnum == (int)SpreadSheetTypeEnum.SQL && configArgs is WriteSqlConfigSpreadSheetArgs,
//                msg: "sql必须传入ReadSQLSpreadsheetArgs");
//            var config = configArgs as WriteSqlConfigSpreadSheetArgs;
//            var dataTable = new DataTable();
//            var typeList = GetWritePropertyList<TData>();

//            var policy = SQLProxy.Instance.GetPolicy(config.SQLType);

//            foreach (var type in typeList)
//                dataTable.Columns.Add(new DataColumn(type.Item1,
//                    policy.IsSQLType(type.Item2.PropertyType) ? type.Item2.PropertyType : typeof(string)));

//            foreach (var item in data)
//            {
//                var row = dataTable.NewRow();
//                foreach (var rowCell in typeList)
//                {
//                    row[rowCell.Item1] = policy.IsSQLType(rowCell.Item2.PropertyType)
//                        ? rowCell.Item2.GetValue(item)
//                        : rowCell.Item2.GetValue(item).ToString();
//                }

//                dataTable.Rows.Add(row);
//            }

//            dataTable.TableName = config.TableName;
//            return dataTable;
//        }

//        #endregion


//        /// <summary>
//        /// 通用写读
//        /// </summary>
//        /// <param name="configArgs">参数配置</param>
//        /// <param name="data">数据</param>
//        /// <param name="type">类型</param>
//        /// <returns>返回转换结果<see cref="string"/> </returns>
//        protected override object WriteModel(WriteConfigSpreadSheetArgs configArgs,
//            KVModel<Dictionary<string, int>, List<List<string>>> data, Type type = null)
//        {
//            throw new NotImplementedException();
//        }

//        private object SQLWrite(WriteConfigSpreadSheetArgs configArgs, DataTable data)
//        {
//            AssertKit.AssertNotNull(configArgs, nameof(configArgs));
//            AssertKit.AssertTrue(
//                configArgs.TypeEnum == (int)SpreadSheetTypeEnum.SQL && configArgs is WriteSqlConfigSpreadSheetArgs,
//                msg: "sql必须传入ReadSQLSpreadsheetArgs");
//            var config = configArgs as WriteSqlConfigSpreadSheetArgs;
//            return SQLProxy.Instance.GetPolicy(config.SQLType).ToSQL(data, config.SQLModel);
//        }

//        /// <summary>
//        /// 简单模型
//        /// </summary>
//        private List<List<string>> GetData(DataTable tableData)
//        {
//            var result = new List<List<string>>();
//            var headLine = new List<string>();
//            foreach (DataColumn item in tableData.Columns)
//                headLine.Add(item.ColumnName);
//            result.Add(headLine);

//            foreach (DataRow row in tableData.Rows)
//            {
//                var line = new List<string>();
//                foreach (var cell in row.ItemArray)
//                    line.Add(cell.ToString());
//                result.Add(line);
//            }

//            return result;
//        }

//        protected override List<List<string>> ReadData(ReadSpreadSheetArgs args)
//        {
//            AssertKit.AssertNotNull(args, nameof(args));
//            AssertKit.AssertTrue(args.TypeEnum == (int)SpreadSheetTypeEnum.SQL && args is ReadSqlSpreadSheetArgs,
//                msg: "sql必须传入ReadSQLSpreadsheetArgs");
//            var configArgs = args as ReadSqlSpreadSheetArgs;
//            var result = SQLProxy.Instance.GetPolicy(configArgs.SQLCommand.SQLType)
//                .ExcuteReadSQL(configArgs.SQLCommand);
//            return GetData(result.TableData).Select(c => c.Select(a => a).ToList()).ToList();
//        }

//        protected override WriteSpreadSheetHandle AppendSpreadSheetData(WriteConfigSpreadSheetArgs configArgs,
//            KVModel<Dictionary<string, int>, List<List<string>>> data, WriteSpreadSheetHandle handle, Type type = null)
//        {
//            throw new NotImplementedException("暂不支持 下一版支持");
//        }
//    }
//}