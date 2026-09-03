using SegaCyrus.ToolKits.Extension.CommonExtension;
using SegaCyrus.ToolKits.Kit;
using SegaCyrus.ToolKits.Model.Common;
using SegaCyrus.ToolKits.Model.Expression;
using SegaCyrus.ToolKits.Model.SpreadSheet.Append;
using SegaCyrus.ToolKits.Model.SpreadSheet.Attributes;
using SegaCyrus.ToolKits.Model.SpreadSheet.Attributes.Excel;
using SegaCyrus.ToolKits.Model.SpreadSheet.Convert;
using SegaCyrus.ToolKits.Model.SpreadSheet.Enums;
using SegaCyrus.ToolKits.Model.SpreadSheet.Excel;
using SegaCyrus.ToolKits.Model.SpreadSheet.Excel.Attributes;
using SegaCyrus.ToolKits.Model.SpreadSheet.Read;
using SegaCyrus.ToolKits.Model.SpreadSheet.Write;
using SegaCyrus.ToolKits.Package.SpreadSheetPackage.Base;
using SegaCyrus.ToolKits.Package.SpreadSheetPackage.Policy.Help;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.Streaming;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace SegaCyrus.ToolKits.Package.SpreadSheetPackage.Policy
{
    [SpreadSheetRegister(SpreadSheetTypeEnum.EXCEL)]
    internal class ExcelSpreadSheetPolicy : SpreadSheetPolicy
    {
        private static ICellStyle GetDefaultBody(ICellStyle bodyCellStyle)
        {
            bodyCellStyle.BorderBottom = BorderStyle.Thin;
            bodyCellStyle.BorderLeft = BorderStyle.Thin;
            bodyCellStyle.BorderRight = BorderStyle.Thin;
            bodyCellStyle.BorderTop = BorderStyle.Thin;
            bodyCellStyle.Alignment = HorizontalAlignment.Left;
            return bodyCellStyle;
        }

        private static ICellStyle GetDefaultHead(ICellStyle headCellStyle)
        {
            headCellStyle.BorderBottom = BorderStyle.Thin;
            headCellStyle.BorderLeft = BorderStyle.Thin;
            headCellStyle.BorderRight = BorderStyle.Thin;
            headCellStyle.BorderTop = BorderStyle.Thin;
            headCellStyle.Alignment = HorizontalAlignment.Center;
            headCellStyle.FillForegroundColor = 29;
            headCellStyle.FillPattern = FillPattern.SolidForeground;
            return headCellStyle;
        }

        /// <summary>
        /// 通用写读
        /// </summary>
        /// <param name="configArgs">参数配置</param>
        /// <param name="data">数据</param>
        /// <param name="type">类型</param>
        /// <returns>返回转换结果<see cref="byte"/> </returns>
        protected override object WriteModel(WriteConfigSpreadSheetArgs configArgs,
            KVModel<Dictionary<string, int>, List<List<string>>> data, Type type = null)
        {
            return AppendSpreadSheetData(configArgs, data, null, type).GetBytes();
        }

        private Dictionary<string, KVModel<BaseExcelDataSource, IDataValidationConstraint>>
            BuildDataValidationConstraints(WriteExcelConfigSpreadSheetArgs args, Type type, IWorkbook workbook)
        {
            var result = new Dictionary<string, KVModel<BaseExcelDataSource, IDataValidationConstraint>>();

            #region 初始化数据源配置池

            var datasourceMapPool
                = args.DataSourceImplList?.ToDictionary(c => c.Key, c => c.Value.GetType().GUID)
                  ?? new Dictionary<string, Guid>();
            var dataValidationPool = new Dictionary<Guid, IDataValidationConstraint>();
            var datasourcePolicyPool = new Dictionary<Guid, BaseExcelDataSource>();
            foreach (var policyItem in args.DataSourceImplList ?? new Dictionary<string, BaseExcelDataSource>())
                datasourcePolicyPool[policyItem.Value.GetType().GUID] = policyItem.Value;

            #endregion

            #region 初始化特性上的数据源

            if (type != null)
            {
                var propertyList = GetWritePropertyList(type)
                    .Select(c => Tuple.Create(c.Item1, c.Item2,
                        c.Item2.GetCustomAttribute<ExcelDataSourceAttribute>(true)))
                    .Where(c => c.Item3 != null)
                    .ToList();
                if (propertyList.NotEmpty())
                {
                    foreach (var datasourceConfigAttrItem in propertyList)
                    {
                        if (datasourceMapPool.ContainsKey(datasourceConfigAttrItem.Item1))
                            continue;
                        if (datasourceConfigAttrItem.Item3.DataSourceType == null)
                        {
                            if (datasourceConfigAttrItem.Item2.PropertyType.IsEnum)
                            {
                                var id = typeof(EnumExcelDataSource).GUID;
                                if (false == datasourcePolicyPool.ContainsKey(id))
                                    datasourcePolicyPool[id] =
                                        (BaseExcelDataSource)ReflectKit.CreateInstance(typeof(EnumExcelDataSource),
                                            new object[] { datasourceConfigAttrItem.Item2.PropertyType });
                                datasourceMapPool[datasourceConfigAttrItem.Item1] = id;
                            }

                            if (datasourceConfigAttrItem.Item2.PropertyType == typeof(bool))
                            {
                                var id = typeof(BoolExcelDataSource).GUID;
                                if (false == datasourcePolicyPool.ContainsKey(id))
                                    datasourcePolicyPool[id] =
                                        (BaseExcelDataSource)ReflectKit.CreateInstance(typeof(BoolExcelDataSource),
                                            new object[] { datasourceConfigAttrItem.Item2.PropertyType });
                                datasourceMapPool[datasourceConfigAttrItem.Item1] = id;
                            }
                        }
                        else
                        {
                            var id = datasourceConfigAttrItem.Item3.DataSourceType.GUID;
                            if (false == datasourcePolicyPool.ContainsKey(id))
                                datasourcePolicyPool[id] = (BaseExcelDataSource)ReflectKit.CreateInstance(
                                    datasourceConfigAttrItem.Item3.DataSourceType,
                                    new object[] { datasourceConfigAttrItem.Item2.PropertyType });
                            datasourceMapPool[datasourceConfigAttrItem.Item1] = id;
                        }
                    }
                }
            }

            #endregion

            #region 构造表格

            var dsValuPool = new List<KVModel<Guid, List<string>>>();
            foreach (var item in datasourcePolicyPool)
                dsValuPool.Add(new KVModel<Guid, List<string>>(item.Key, item.Value.DataSourcePool()));
            dsValuPool = dsValuPool.OrderByDescending(c => c.Value.Count).ToList();

            if (dsValuPool.IsEmpty())
                return result;

            var size = dsValuPool.Max(c => c.Value.Count);

            var sheetName = args.DropDataSourceSheetName;
            var sheet2 = workbook.CreateSheet(sheetName);
            if (args.ExcelType == ExcelTypeEnum.Xls)
                sheet2.TabColorIndex = 10;
            else if (args.ExcelType == ExcelTypeEnum.ExtXlsx)
            {
                var t = sheet2 as SXSSFSheet;
#if NET8_0_OR_GREATER
                t.TabColor = new XSSFColor(SixLabors.ImageSharp.Color.DarkRed);
                workbook.SetSheetHidden(workbook.NumberOfSheets - 1, SheetVisibility.VeryHidden);
#else
                t.TabColor = new XSSFColor(new byte[] { 255, 0, 0 });
#endif
            }
            else
            {
                var t = sheet2 as XSSFSheet;
#if NET8_0_OR_GREATER
                t.TabColor = new XSSFColor(SixLabors.ImageSharp.Color.DarkRed);
#else
                t.SetTabColor(10);
#endif
            }

            if (args.HideDataSourceSheet)
#if NET8_0_OR_GREATER
                workbook.SetSheetHidden(workbook.NumberOfSheets - 1,SheetVisibility.VeryHidden);
#else
                workbook.SetSheetHidden(workbook.NumberOfSheets - 1, SheetState.VeryHidden);
#endif

            for (var index = 0; index < size; ++index)
            {
                var row = sheet2.CreateRow(index);
                for (var i = 0; i < dsValuPool.Count; ++i)
                {
                    if (dsValuPool[i].Value.Count > index)
                        row.CreateCell(i).SetCellValue(dsValuPool[i].Value[index]);
                }
            }

#endregion

            var name = BuildDefaultColumnName(dsValuPool.Count);

            for (var i = 0; i < dsValuPool.Count; ++i)
            {
                var rangeFormula = sheetName + $"!${name[i]}$1:${name[i]}$" + dsValuPool[i].Value.Count;
                if (args.ExcelType == ExcelTypeEnum.Xls)
                {
                    var range = workbook.CreateName();
                    range.RefersToFormula = rangeFormula;
                    range.NameName = $"{sheetName}{i}";
                    dataValidationPool[dsValuPool[i].Key] = DVConstraint.CreateFormulaListConstraint(range.NameName);
                }
                else
                    dataValidationPool[dsValuPool[i].Key] = new XSSFDataValidationConstraint(3, rangeFormula);
            }

            #region 构造返回结果

            foreach (var item in datasourceMapPool)
            {
                result[item.Key] =
                    new KVModel<BaseExcelDataSource, IDataValidationConstraint>(datasourcePolicyPool[item.Value],
                        dataValidationPool[item.Value]);
            }

            #endregion

            return result;
        }

        private void SetCellDropdownList(ExcelTypeEnum excelType, IDataValidationConstraint constraint, ISheet sheet,
            int columnIndex, int start, int end, string title, string message)
        {
            var region = new CellRangeAddressList(start, end, columnIndex, columnIndex);
            if (excelType == ExcelTypeEnum.ExtXlsx || excelType == ExcelTypeEnum.Xlsx)
            {
                var helper = sheet.GetDataValidationHelper();
                var typesValidation = helper.CreateValidation(constraint, region);
                typesValidation.CreateErrorBox(title, message);
                typesValidation.ShowPromptBox = true;
                sheet.AddValidationData(typesValidation);
            }
            else
            {
                var validate = new HSSFDataValidation(region, constraint);
                validate.CreateErrorBox(title, message);
                validate.ShowPromptBox = true;
                sheet.AddValidationData(validate);
            }
        }

        protected override List<List<string>> ReadData(ReadSpreadSheetArgs args)
        {
            AssertKit.AssertTrue(args.TypeEnum == (int)SpreadSheetTypeEnum.EXCEL && args is ReadExcelSpreadSheetArgs,
                msg: "EXCEL必须传入ReadExcelSpreadSheetArgs");
            var readArgs = args as ReadExcelSpreadSheetArgs;

            var result = new List<List<string>>();
            IWorkbook workbook = null;
            using (var memoryStream = new MemoryStream(readArgs.ExcelData))
            {
                workbook = WorkbookFactory.Create(memoryStream, ImportOption.NONE);
            }

            AssertKit.AssertGreater(workbook.NumberOfSheets, 0, "Excel的Sheet页数量");

            var readSheetCount = readArgs.ReadSheetIndexs ?? new HashSet<int>() { 0 };

            List<string> headLine = null;
            for (var i = 0; i < workbook.NumberOfSheets; ++i)
            {
                if (readSheetCount.NotEmpty() && false == readSheetCount.Contains(i))
                    continue;
                var sheet = workbook.GetSheetAt(i);

                for (var rowCount = 0; rowCount < sheet.PhysicalNumberOfRows; ++rowCount)
                {
                    var rowItem = new List<string>();
                    var row = sheet.GetRow(rowCount);
                    if (row == null)
                        continue;

                    for (var colCount = 0; colCount < row.LastCellNum; ++colCount)
                    {
                        var excelCell = row.GetCell(colCount, MissingCellPolicy.RETURN_NULL_AND_BLANK);
                        rowItem.Add(excelCell?.ToString() ?? string.Empty);
                    }

                    if (readArgs.HasHead && rowCount == 0 && headLine == null)
                        headLine = rowItem;
                    else if (readArgs.SkipEmptyRow == false || rowItem.Exists(c => c != string.Empty))
                        result.Add(rowItem);
                }
            }

            if (readArgs.HasHead)
                result.Insert(0, headLine);
            return result;
        }

        protected override WriteSpreadSheetHandle AppendSpreadSheetData(WriteConfigSpreadSheetArgs configArgs,
            KVModel<Dictionary<string, int>, List<List<string>>> data, WriteSpreadSheetHandle handle, Type type = null)
        {
            AssertKit.AssertTrue(handle == null || handle.TypeEnum == (int)SpreadSheetTypeEnum.EXCEL,
                msg: "handle类型不正确");
            var args = configArgs is WriteExcelConfigSpreadSheetArgs
                ? configArgs as WriteExcelConfigSpreadSheetArgs
                : new WriteExcelConfigSpreadSheetArgs();
            return AppendSpreadSheetData(args, data, handle as WriteExcelSpreadSheetHandle, type);
        }

        private WriteExcelSpreadSheetHandle AppendSpreadSheetData(WriteExcelConfigSpreadSheetArgs args,
            KVModel<Dictionary<string, int>, List<List<string>>> data, WriteExcelSpreadSheetHandle handle,
            Type type = null)
        {
            if (handle == null || handle.GetWorkBook() == null)
            {
                IWorkbook excelWorkBook;
                if (args.ExcelType == ExcelTypeEnum.Xlsx)
                    excelWorkBook = new XSSFWorkbook();
                else if (args.ExcelType == ExcelTypeEnum.Xls)
                    excelWorkBook = new HSSFWorkbook();
                else if (args.ExcelType == ExcelTypeEnum.ExtXlsx)
                    excelWorkBook =
                        new SXSSFWorkbook(AssertKit.AssertPositive(args.FlushDiskRowCount,
                            nameof(args.FlushDiskRowCount)));
                else
                    throw new ArgumentException($"不支持的文件类型,{args.ExcelType}");
                handle = new WriteExcelSpreadSheetHandle();
                handle.SetWorkBook(excelWorkBook);
                handle.ExcelType = args.ExcelType;
            }
            else
            {
                AssertKit.AssertTrue(handle.ExcelType == args.ExcelType, "参数非法");
                AssertKit.AssertNotNull(handle.GetWorkBook(), "参数非法");
            }


            List<string> headLine = null;
            var tmpData = data.Value.ToList();
            if (args.AddHead)
            {
                headLine = tmpData.First();
                tmpData.RemoveAt(0);
            }

            #region cell style

            var headCellStyle = args.HeadCellStyle(GetDefaultHead(handle.GetWorkBook().CreateCellStyle()),
                handle.GetWorkBook().CreateFont());

            var columnCellStyleSet = new Dictionary<int, ICellStyle>();
            for (int i = 0; i < headLine.Count; ++i)
                columnCellStyleSet[i] = args.BodyCellStyle(GetDefaultBody(handle.GetWorkBook().CreateCellStyle()),
                handle.GetWorkBook().CreateFont());

            #endregion


            #region datasource

            var dataValidatePool = handle.DataValidatePoolSheetMapVaild.ContainsKey(args.DropDataSourceSheetName)
                ? handle.DataValidatePoolSheetMapVaild[args.DropDataSourceSheetName]
                : BuildDataValidationConstraints(args, type, handle.GetWorkBook());
            handle.DataValidatePoolSheetMapVaild[args.DropDataSourceSheetName] = dataValidatePool;

            #endregion

            var sheetName = args.SheetName;

            var existSheet = false;

            var sheet = handle.GetWorkBook().GetSheet(sheetName);
            if (sheet != null)
                existSheet = true;
            else
                sheet = handle.GetWorkBook().CreateSheet(sheetName);
            var rowNums = sheet.LastRowNum;
            rowNums += (rowNums == 0 ? 0 : 1);
            var begin = rowNums;

            if (args.AddHead && existSheet == false)
            {
                var columnNums = 0;
                var row = sheet.CreateRow(rowNums++);
                foreach (var columnItem in headLine)
                {
                    var cellData = row.CreateCell(columnNums++);
                    cellData.SetCellValue(columnItem);
                    cellData.CellStyle = headCellStyle;
                }
            }

            foreach (var rowItem in tmpData)
            {
                var columnNums = 0;
                var row = sheet.CreateRow(rowNums++);
                foreach (var columnItem in rowItem)
                {
                    var cellData = row.CreateCell(columnNums);
                    var t = columnItem;
                    if (t == null)
                        cellData.SetBlank();
                    else
                        cellData.SetCellValue(t);
                    cellData.SetCellType(CellType.String);
                    cellData.CellStyle = columnCellStyleSet[columnNums++];
                }
            }

            if (args.AddHead && existSheet == false)
            {
                if (type != null && data.Key != null)
                {
                    var typeAttribute = GetWritePropertyList(type)
                        .Where(c => c.Item3 != null)
                        .Where(c => c.Item3 is ExcelColumnDefineAttribute).ToList();
                    if (typeAttribute.Count > 0)
                    {
                        var orderMap = data.Key;
                        var autoSizeColumnSet = new HashSet<int>();
                        foreach (var item in typeAttribute)
                        {
                            var attributeConfig = item.Item3 as ExcelColumnDefineAttribute;
                            if (orderMap.TryGetValue(item.Item1, out var order) == false)
                                continue;
                            if (attributeConfig.ColumnWidth > 0)
                                sheet.SetColumnWidth(order, attributeConfig.ColumnWidth);
                            if (attributeConfig.AutoSize)
                                autoSizeColumnSet.Add(order);
                            columnCellStyleSet[order].WrapText = attributeConfig.AutoWrapText;
                        }

                        if (autoSizeColumnSet.Any()) //自适应宽度
                        {
                            if (args.ExcelType == ExcelTypeEnum.ExtXlsx)
                            {
                                var sheetSxxs = sheet as SXSSFSheet;
                                sheetSxxs.TrackAllColumnsForAutoSizing();
                            }

                            foreach (var columnIndex in autoSizeColumnSet)
                                sheet.AutoSizeColumn(columnIndex);
                        }
                    }
                }

                else if (args is DynamicDataWriteExcelConfigSpreadSheetArgs p && p != null && p.AutoColumnSize) //自适应宽度
                {
                    if (args.ExcelType == ExcelTypeEnum.ExtXlsx)
                    {
                        var sheetSxxs = sheet as SXSSFSheet;
                        sheetSxxs.TrackAllColumnsForAutoSizing();
                    }

                    for (var columnIndex = 0; columnIndex < headLine.Count; columnIndex++)
                        sheet.AutoSizeColumn(columnIndex);
                }
                if (args.ForzenHead) //首行冻结
                    sheet.CreateFreezePane(0, 1, 0, 1);
                if (args.SetHeadFilter) //首行筛选
                    sheet.SetAutoFilter(new CellRangeAddress(0, 0, 0, headLine.Count - 1));
            }

            if (args.ForzenFirstColumn)
            {
                //首行冻结
                sheet.CreateFreezePane(1, 0, 1, 0);
            }


            if (dataValidatePool.NotEmpty())
            {
                foreach (var columnItem in data.Key)
                {
                    if (dataValidatePool.TryGetValue(columnItem.Key, out var dataValidate) &&
                        dataValidate != null)
                    {
                        var t = existSheet == false ? (args.AddHead ? 1 : 0) : begin;
                        SetCellDropdownList(args.ExcelType, dataValidate.Value, sheet, columnItem.Value,
                            t, tmpData.Count + t,
                            dataValidate.Key.CheckBoxTitle(columnItem.Key),
                            dataValidate.Key.CheckBoxMessage(columnItem.Key));
                    }
                }
            }

            //XLS版本移动后数据源可能有问题 为了表现一致  此处的sheet页暂不移动
            if (dataValidatePool.NotEmpty()) // && args.ExcelType==ExcelTypeEnum.Xlsx)
            //{
            //    excelWorkBook.SetSheetOrder(excelWorkBook.GetSheetAt(0).SheetName, excelWorkBook.NumberOfSheets - 1);
                handle.GetWorkBook().SetActiveSheet(1);
            //}

            Resize(handle, sheet, args.InternalRowHeight, args.InternalColumnWidth);

            handle.SetWorkBook(args.FinishWriteExcel(handle.GetWorkBook()));

            return handle;
        }

        private void Resize(WriteExcelSpreadSheetHandle handle, ISheet sheet, Dictionary<int, short> rowHeight,
            Dictionary<int, int> columnWidth)
        {
            if (columnWidth.NotEmpty())
            {
                foreach (var column in columnWidth)
                {
                    sheet.SetColumnWidth(column.Key, column.Value);
                }
            }

            if (rowHeight.NotEmpty())
            {
                var maxRow = sheet.LastRowNum;
                foreach (var row in rowHeight)
                {
                    if (row.Key >= maxRow)
                        continue;
                    var rowItem = sheet.GetRow(row.Key);
                    if (rowItem == null && handle.ExcelType == ExcelTypeEnum.ExtXlsx)
                        throw new ArgumentException($"当前操作行已经被刷新到磁盘中,请调整目标列或者刷盘大小,目标行{row.Key}");
                    sheet.GetRow(row.Key).Height = row.Value;
                    sheet.GetRow(row.Key).HeightInPoints = row.Value * 20;
                }
            }
        }
    }
}