using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using SegaCyrus.ToolKits.Extension.CodeExtension;
using SegaCyrus.ToolKits.Extension.CommonExtension;
using SegaCyrus.ToolKits.Kit;
using SegaCyrus.ToolKits.Model.Common;
using SegaCyrus.ToolKits.Model.SpreadSheet.Append;
using SegaCyrus.ToolKits.Model.SpreadSheet.Attributes;
using SegaCyrus.ToolKits.Model.SpreadSheet.Enums;
using SegaCyrus.ToolKits.Model.SpreadSheet.Handle;
using SegaCyrus.ToolKits.Model.SpreadSheet.Read;
using SegaCyrus.ToolKits.Model.SpreadSheet.Write;
using SegaCyrus.ToolKits.Package.SpreadSheetPackage.Base;

namespace SegaCyrus.ToolKits.Package.SpreadSheetPackage.Policy
{
    [SpreadSheetRegister(SpreadSheetTypeEnum.CSV)]
    internal class CsvSpreadSheetPolicy : SpreadSheetPolicy
    {
        private const int BlockSize = 1 << 15;

        protected override List<List<string>> ReadData(ReadSpreadSheetArgs args)
        {
            List<List<string>> tmpResult = null;
            AssertKit.AssertNotNull(args, nameof(args));
            AssertKit.AssertTrue(args.TypeEnum == (int)SpreadSheetTypeEnum.CSV, msg: "必须操作CSV数据");
            if (args is ReadCsvSpreadSheetArgs configV1)
                tmpResult = ReadCSV(configV1);
            else if (args is ReadBigCsvSpreadSheetArgs configV2)
                tmpResult = ReadBigCSV(configV2);
            else
                throw new ArgumentException("未知实现");
            return tmpResult;
        }


        /// <summary>
        /// 通用写读
        /// </summary>
        /// <param name="configArgs">参数配置</param>
        /// <param name="data">数据</param>
        /// <param name="type">类型</param>
        /// <returns>返回转换结果<see cref="string"/> </returns>
        protected override object WriteModel(WriteConfigSpreadSheetArgs configArgs,
            KVModel<Dictionary<string, int>, List<List<string>>> data, Type type = null)
        {
            var handle = AppendSpreadSheetData(configArgs, data, null, type);
            return handle.GetBulderHandle().ToString();
        }

        private static string GetCSVCellData(string data)
        {
            var stringBuilder = new StringBuilder(data.Length << 1);
            if (data.IsEmpty())
                return string.Empty;
            var prefixEnable = false;
            foreach (var item in data)
            {
                if (item == ',' || item == '\"' || item == '\n' || item == '\r')
                    prefixEnable = true;
                if (item == '\"')
                    stringBuilder.Append('\"');
                stringBuilder.Append(item);
            }

            if (prefixEnable)
                return new StringBuilder(stringBuilder.Length + 3).Append('\"').Append(stringBuilder).Append('\"').ToString();
            return stringBuilder.ToString();
        }

        #region BigFile

        private List<List<string>> ReadBigCSV(ReadBigCsvSpreadSheetArgs configV2)
        {
            AssertKit.AssertEQGreater(configV2.StartIndex, 0, msg: "start 必须大于 0");
            AssertKit.AssertEQGreater(configV2.EndIndex, 0, msg: "end 必须大于 0");
            AssertKit.AssertEQLesser(configV2.StartIndex, configV2.EndIndex, msg: "start 必须小于 end");

            if (configV2.RowSpData == null)
                InitBigCSV(configV2);

            var data = ReadBigLine(configV2, configV2.StartIndex, configV2.EndIndex) ?? new List<List<string>>();

            if (configV2.HasHead)
            {
                var headLine = ReadBigLine(configV2, 0, 0).FirstOrDefault();
                data.Insert(0, headLine);
            }

            return data;
        }

        private List<List<string>> ReadBigLine(ReadBigCsvSpreadSheetArgs args, int start, int end)
        {
            AssertKit.AssertEQGreater(start, 0, msg: "start 必须大于 0");
            AssertKit.AssertEQGreater(end, 0, msg: "end 必须大于 0");
            AssertKit.AssertEQLesser(start, end, msg: "start 必须小于 end");
            var startIndex = start == 0 ? -1 : args.RowSpData[start - 1];
            var endIndex = args.RowSpData[end];
            var rangeCount = (int)(endIndex - startIndex);
            var sectionBytes = args.CsvBigFileStream.ReadOffset(startIndex + 1, rangeCount, out _);
            var sectionString = sectionBytes.UTF8ToString();
            return ReadCSV(sectionString);
        }

        private void InitBigCSV(ReadBigCsvSpreadSheetArgs args)
        {
            args.RowSpData = new List<long>();
            long offset = 0;

            var existBodyHead = false;
            var csvPartitionBytes = new byte[BlockSize];

            while (true)
            {
                var size = args.CsvBigFileStream.ReadOffset(ref csvPartitionBytes, offset, BlockSize);
                if (size == 0)
                    return;
                var csvPartiton = size < BlockSize
                    ? csvPartitionBytes.Take(size).ToArray().UTF8ToString()
                    : csvPartitionBytes.UTF8ToString();
                var count = csvPartiton.Length;
                for (var i = 0; i < count; ++i)
                {
                    if (csvPartiton[i] == '\"')
                    {
                        if (existBodyHead == false)
                            existBodyHead = true;
                        else if (i + 1 < csvPartiton.Length)
                        {
                            ++i;
                            if (csvPartiton[i] == ',')
                                existBodyHead = false;
                            else if (csvPartiton[i] == '\r' && csvPartiton[i + 1] == '\n')
                            {
                                ++i;
                                existBodyHead = false;
                                args.RowSpData.Add(offset + i);
                            }
                            else if (csvPartiton[i] == '\n')
                            {
                                existBodyHead = false;
                                args.RowSpData.Add(offset + i);
                            }
                        }
                    }
                    else if (false == existBodyHead)
                    {
                        if (csvPartiton[i] == '\n')
                        {
                            existBodyHead = false;
                            args.RowSpData.Add(offset + i);
                        }
                        else if (csvPartiton[i] == '\r' && csvPartiton[i + 1] == '\n')
                        {
                            ++i;
                            existBodyHead = false;
                            args.RowSpData.Add(offset + i);
                        }
                    }
                }

                offset += size;
            }
        }

        #endregion

        #region CSVRead

        private List<List<string>> ReadCSV(ReadCsvSpreadSheetArgs configV1)
        {
            return ReadCSV(configV1.CsvData);
        }

        private List<List<string>> ReadCSV(string csvData)

        {
            var result = new List<List<string>>();
            var resultItem = new List<string>();

            var existBodyHead = false;
            if (string.IsNullOrEmpty(csvData))
                return result;
            var stringBuilder = new StringBuilder();
            for (var i = 0; i < csvData.Length; ++i)
            {
                if (csvData[i] == '\"')
                {
                    if (existBodyHead == false)
                        existBodyHead = true;
                    else if (i + 1 < csvData.Length)
                    {
                        ++i;
                        if (csvData[i] == ',')
                        {
                            existBodyHead = false;
                            resultItem.Add(stringBuilder.ToString());
                            stringBuilder.Clear();
                        }
                        else if (csvData[i] == '\r' && csvData[i + 1] == '\n')
                        {
                            existBodyHead = false;
                            resultItem.Add(stringBuilder.ToString());
                            stringBuilder.Clear();
                            i++;
                            result.Add(resultItem);
                            resultItem = new List<string>();
                        }

                        else if (csvData[i] == '\n')
                        {
                            existBodyHead = false;
                            resultItem.Add(stringBuilder.ToString());
                            stringBuilder.Clear();
                            result.Add(resultItem);
                            resultItem = new List<string>();
                        }

                        else
                        {
                            stringBuilder.Append(csvData[i]);
                        }
                    }
                }
                else if (false == existBodyHead)
                {
                    if (csvData[i] == ',')
                    {
                        resultItem.Add(stringBuilder.ToString());
                        stringBuilder.Clear();
                    }
                    else if (csvData[i] == '\n')
                    {
                        resultItem.Add(stringBuilder.ToString());
                        stringBuilder.Clear();
                        result.Add(resultItem);
                        resultItem = new List<string>();
                    }
                    else if (csvData[i] == '\r' && csvData[i + 1] == '\n')
                    {
                        ++i;
                        resultItem.Add(stringBuilder.ToString());
                        stringBuilder.Clear();
                        result.Add(resultItem);
                        resultItem = new List<string>();
                    }
                    else
                        stringBuilder.Append(csvData[i]);
                }
                else
                    stringBuilder.Append(csvData[i]);
            }

            return result;
        }

        protected override WriteSpreadSheetHandle AppendSpreadSheetData(WriteConfigSpreadSheetArgs configArgs,
            KVModel<Dictionary<string, int>, List<List<string>>> data, WriteSpreadSheetHandle handle, Type type = null)
        {
            AssertKit.AssertTrue(null == handle || handle is WriteCSVSpreadSheetHandle, "参数错误");
            return AppendSpreadSheetData(configArgs, data, handle as WriteCSVSpreadSheetHandle, type);
        }


        private WriteCSVSpreadSheetHandle AppendSpreadSheetData(WriteConfigSpreadSheetArgs configArgs,
            KVModel<Dictionary<string, int>, List<List<string>>> data, WriteCSVSpreadSheetHandle handle,
            Type type = null)
        {
            handle = handle ?? new WriteCSVSpreadSheetHandle();
            if (data == null)
                return handle;

            var csvRowData = data.Value ?? new List<List<string>>();
            if (csvRowData.IsEmpty())
                return handle;

            if (configArgs.AddHead && handle.HasWrited)
                csvRowData.RemoveAt(0);

            if (handle.IsStream())
            {
                using (var tmp = new StreamWriter(handle.GetStreamHandle()))
                {
                    foreach (var item in csvRowData)
                    {
                        var dataLine = new List<string>();
                        foreach (var cell in item)
                            dataLine.Add(GetCSVCellData(cell));
                        tmp.WriteLine(string.Join(",", item));
                    }
                }
            }
            else
            {
                foreach (var item in csvRowData)
                {
                    var dataLine = new List<string>();
                    foreach (var cell in item)
                        dataLine.Add(GetCSVCellData(cell));
                    handle.GetBulderHandle().AppendLine(string.Join(",", item));
                }
            }

            return handle;
        }

        #endregion
    }
}