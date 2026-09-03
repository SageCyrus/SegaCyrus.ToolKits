using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using SegaCyrus.ToolKits.Internal;
using SegaCyrus.ToolKits.Model.Common;
using SegaCyrus.ToolKits.Model.SpreadSheet.Append;
using SegaCyrus.ToolKits.Model.SpreadSheet.Attributes;
using SegaCyrus.ToolKits.Model.SpreadSheet.Enums;
using SegaCyrus.ToolKits.Model.SpreadSheet.Read;
using SegaCyrus.ToolKits.Model.SpreadSheet.Write;
using SegaCyrus.ToolKits.Package.SpreadSheetPackage.Base;

namespace SegaCyrus.ToolKits.Package.SpreadSheetPackage.Policy
{
    [SpreadSheetRegister(SpreadSheetTypeEnum.FormatString)]
    internal class FormatStringSpreadSheetPolicy : SpreadSheetPolicy
    {
        private List<int> GetMaxSizeColumn(WriteStringConfigSpreadSheetArgs config, List<List<string>> data)
        {
            var dataTranspose = MathExtension.Transpose(data);
            return dataTranspose.Select(c => Math.Min(config.TruncateDataLen, c.Max(x => GetStringCount(x)))).ToList();
        }

        private string BuildCrossingLine(List<int> columnSizes, string leftVertex, string spliteVertexEdge,
            string rightVertex, char horizontalEdge)
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.Append(leftVertex);

            stringBuilder
                .Append(string.Join(spliteVertexEdge, columnSizes.Select(c => new string(horizontalEdge, c))));

            stringBuilder.Append(rightVertex);
            return stringBuilder.ToString();
        }

        private string BuildTopCrossingLine(WriteStringConfigSpreadSheetArgs config, List<int> columnSizes)
        {
            return BuildCrossingLine(columnSizes, config.TopLeftVertex, config.TopSpliteVertexEdge,
                config.TopRightVertex, config.HorizontalEdge);
        }

        private string BuildBottomCrossingLine(WriteStringConfigSpreadSheetArgs config, List<int> columnSizes)
        {
            return BuildCrossingLine(columnSizes, config.BottomLeftVertex, config.BottomSpliteVertexEdge,
                config.BottomRightVertex, config.HorizontalEdge);
        }

        private string BuildMidCrossingLine(WriteStringConfigSpreadSheetArgs config, List<int> columnSizes)
        {
            return BuildCrossingLine(columnSizes, config.LeftSpliteVertexEdge, config.MiddleSpliteVertexEdge,
                config.RightSpliteVertexEdge, config.HorizontalEdge);
        }

        private int GetStringCount(string text)
        {
            GetConsoleLength('（');
            if (string.IsNullOrEmpty(text))
                return 0;
            return text.Sum(c => GetConsoleLength(c));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int GetConsoleLength(char c)
        {
            return CharUnicodeInfo.GetUnicodeCategory(c) is UnicodeCategory.OtherLetter ? 2 : (c > 255 ? 2 : 1);
        }

        private string FormatData(string data, int count, string resverStr)
        {
            resverStr = resverStr ?? "...";

            if (string.IsNullOrEmpty(data))
                return new string(' ', count);

            data = data.Replace("\n", "\\n").Replace("\t", "\\t");
            Redo:

            var stringBuilder = new StringBuilder();
            var consoleLength = GetStringCount(data);

            if (consoleLength > count)
            {
                var i = 0;
                var reCount = count - resverStr.Length;
                foreach (var c in data)
                {
                    i += GetConsoleLength(c);
                    if (i < reCount)
                        stringBuilder.Append(c);
                    else
                    {
                        stringBuilder.Append(resverStr);
                        data = stringBuilder.ToString();
                        goto Redo;
                    }
                }
            }
            else if (consoleLength < count)
            {
                stringBuilder.Append(data).Append(' ', count - consoleLength);
                return stringBuilder.ToString();
            }

            return data;
        }

        private string BuildDataLine(WriteStringConfigSpreadSheetArgs config, List<string> data, List<int> columnSizes)
        {
            var stringBuilder = new StringBuilder();

            var tmp = new List<KVModel<string, int>>(data.Count);
            for (var i = 0; i < data.Count; i++)
                tmp.Add(new KVModel<string, int>(data[i], columnSizes[i]));

            if (config.TruncateText)
            {
                stringBuilder.Append(config.VerticalEdge);
                stringBuilder.Append(string.Join(config.VerticalEdge,
                    tmp.Select(c => FormatData(c.Key, c.Value, "..."))));
                stringBuilder.Append(config.VerticalEdge);
            }
            else
            {
                var spTmpData = data.Select(c => new List<string>()).ToList();
                for (var i = 0; i < data.Count; i++)
                {
                    var sbBuilder = new StringBuilder();
                    var consoleLength = GetStringCount(data[i]);
                    if (consoleLength > columnSizes[i])
                    {
                        var currentStringSize = 0;
                        foreach (var c in data[i])
                        {
                            currentStringSize += GetConsoleLength(c);
                            if (currentStringSize < columnSizes[i])
                            {
                                sbBuilder.Append(c);
                            }
                            else
                            {
                                var tmpstr = sbBuilder.ToString();
                                var tmplen = GetStringCount(tmpstr);
                                currentStringSize = 0;
                                if (tmplen < columnSizes[i])
                                {
                                    spTmpData.ElementAt(i).Add(tmpstr + new string(' ', columnSizes[i] - tmplen));
                                }
                                else
                                    spTmpData.ElementAt(i).Add(tmpstr);

                                sbBuilder.Clear().Append(c);
                            }
                        }

                        if (sbBuilder.Length > 0)
                        {
                            var tes = sbBuilder.ToString();
                            var te = GetStringCount(tes);
                            if (te < columnSizes[i])
                            {
                                sbBuilder.Append(' ', columnSizes[i] - te);
                                spTmpData.ElementAt(i).Add(sbBuilder.ToString());
                            }
                            else
                            {
                                spTmpData.ElementAt(i).Add(tes);
                            }

                            sbBuilder.Clear();
                        }
                    }
                    else if (consoleLength < columnSizes[i])
                    {
                        sbBuilder.Append(data[i]).Append(' ', columnSizes[i] - consoleLength);
                        spTmpData.ElementAt(i).Add(sbBuilder.ToString());
                    }
                    else
                        spTmpData.ElementAt(i).Add(data[i]);
                }


                var maxData = spTmpData.Max(c => c.Count);
                if (1 == maxData)
                {
                    stringBuilder.Append(config.VerticalEdge);
                    stringBuilder.Append(string.Join(config.VerticalEdge, spTmpData.Select(c => c.First())));
                    stringBuilder.Append(config.VerticalEdge);
                }
                else
                {
                    var tmDt = spTmpData.Select(c => string.Empty).ToList();
                    for (var j = 0; j < maxData; ++j)
                    {
                        for (var ii = 0; ii < spTmpData.Count; ++ii)
                            tmDt[ii] = spTmpData[ii].Count > j
                                ? spTmpData[ii].ElementAt(j)
                                : new string(' ', columnSizes[ii]);

                        stringBuilder.Append(config.VerticalEdge);
                        stringBuilder.Append(string.Join(config.VerticalEdge, tmDt));
                        stringBuilder.Append(config.VerticalEdge);
                        if (j + 1 != maxData) stringBuilder.AppendLine();
                    }
                }
            }

            return stringBuilder.ToString();
        }

        private string BuildFormatTable(WriteStringConfigSpreadSheetArgs config, List<List<string>> data)
        {
            var stringBuilder = new StringBuilder();
            var maxSizes = GetMaxSizeColumn(config, data);
            stringBuilder.AppendLine(BuildTopCrossingLine(config, maxSizes));
            if (config.WithHorizontalEdge)
            {
                stringBuilder.AppendLine(string.Join("\r\n" + BuildMidCrossingLine(config, maxSizes) + "\r\n",
                    data.Select(c => BuildDataLine(config, c, maxSizes))));
            }
            else if (config.AddHead)
            {
                stringBuilder
                    .AppendLine(BuildDataLine(config, data.First(), maxSizes))
                    .AppendLine(BuildMidCrossingLine(config, maxSizes))
                    .AppendLine(string.Join("\r\n", data.Skip(1).Select(c => BuildDataLine(config, c, maxSizes))));
            }
            else
                stringBuilder.AppendLine(BuildMidCrossingLine(config, maxSizes))
                    .AppendLine(string.Join("\r\n", data.Select(c => BuildDataLine(config, c, maxSizes))));

            stringBuilder.AppendLine(BuildBottomCrossingLine(config, maxSizes));

            return stringBuilder.ToString();
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
            var config = configArgs as WriteStringConfigSpreadSheetArgs;
            config = config ?? new WriteStringConfigSpreadSheetArgs();
            return BuildFormatTable(config, data.Value);
        }

        protected override List<List<string>> ReadData(ReadSpreadSheetArgs args)
        {
            throw new NotImplementedException("这个数据别想了,不可能支持读取的");
        }

        protected override WriteSpreadSheetHandle AppendSpreadSheetData(WriteConfigSpreadSheetArgs configArgs,
            KVModel<Dictionary<string, int>, List<List<string>>> data, WriteSpreadSheetHandle handle, Type type = null)
        {
            throw new NotImplementedException("暂不支持 下一版支持");
        }
    }
}