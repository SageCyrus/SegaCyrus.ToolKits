//using SegaCyrus.ToolKits.Kit.EasyExtension.EasyCodeKit;
//using SegaCyrus.ToolKits.Model.SpreadSheet.Attributes;
//using SegaCyrus.ToolKits.Model.SpreadSheet.Enums;
//using SegaCyrus.ToolKits.Model.SpreadSheet.Read;
//using SegaCyrus.ToolKits.Model.SpreadSheet.Write;
//using SegaCyrus.ToolKits.Package.SpreadSheetPackage.Base;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//namespace SegaCyrus.ToolKits.Package.SpreadSheetPackage.Policy
//{
//    internal class StringSpreadSheetPolicy : SpreadSheetPolicy
//    {
//        private List<string> WriteListString(List<List<string>> data, bool withHead, int maxLen = int.MaxValue,
//            char lineChar = '-',
//            char boardChar = '|', string splitString = " | ")
//        {
//            var result = new List<string>();
//            var columnMaxSize = new Dictionary<int, int>();

//            var writeData = data.ToList();
//            List<string> headLine = data.First();

//            foreach (var row in writeData)
//            {
//                for (int i = 0; i < row.Count; ++i)
//                {
//                    var tmp = row.ElementAt(i);
//                    if (tmp == null)
//                        continue;
//                    if (Encoding.Default.GetByteCount(tmp) > maxLen)
//                        tmp = $"{tmp.ToCharArray().Take(maxLen - 3).ToArray().CharsToString()}...";
//                    //tmp = $"{tmp.Substring(0, maxLen - 3)}...";
//                    if (columnMaxSize.ContainsKey(i))
//                        columnMaxSize[i] = Math.Max(columnMaxSize[i], Encoding.Default.GetByteCount(tmp));
//                    else
//                        columnMaxSize.Add(i, Encoding.Default.GetByteCount(tmp));
//                }
//            }

//            var titleList = new List<string>();
//            for (int i = 0; i < headLine.Count; ++i)
//            {
//                var tmp = headLine.ElementAt(i);
//                if (tmp.Length > maxLen)
//                {

//                    tmp = $"{tmp.ToCharArray().Take(maxLen - 3).ToArray().CharsToString()}...";
//                    //tmp = $"{tmp.Substring(0, maxLen - 3)}...";
//                }
//                else
//                {
//                    var t = Encoding.Default.GetByteCount(tmp);
//                    if (columnMaxSize[i] > t)
//                        tmp = tmp.PadRight(columnMaxSize[i] + tmp.Length - t);
//                }

//                titleList.Add(tmp);
//            }

//            var titleString = string.Join(splitString, titleList);

//            if (withHead)
//            {
//                writeData.RemoveAt(0);
//                result.Add($"{boardChar}{new string(lineChar, Encoding.Default.GetByteCount(titleString))}{boardChar}");
//                result.Add($"{boardChar}{titleString}|");
//                result.Add($"{boardChar}{new string(lineChar, Encoding.Default.GetByteCount(titleString))}{boardChar}");
//            }
//            else
//            {
//                result.Add($"{boardChar}{new string(lineChar, Encoding.Default.GetByteCount(titleString))}{boardChar}");
//            }

//            foreach (var item in writeData)
//            {
//                var rowData = new List<string>();
//                for (int i = 0; i < item.Count; ++i)
//                {
//                    var tmp = item.ElementAt(i) ?? string.Empty;
//                    if (tmp.Length > maxLen)
//                    {
//                        tmp = $"{tmp.ToCharArray().Take(maxLen - 3).ToArray().CharsToString()}...";
//                        //tmp = $"{tmp.Substring(0, maxLen - 3)}...";
//                    }
//                    else
//                    {
//                        var t = tmp.Length;
//                        if (columnMaxSize[i] > t)
//                            tmp = tmp.PadRight(columnMaxSize[i] + tmp.Length - t);
//                    }

//                    rowData.Add(tmp);
//                }

//                var rowStrData = string.Join(splitString, rowData);
//                result.Add($"{boardChar}{rowStrData}{boardChar}");
//            }

//            result.Add($"{boardChar}{new string(lineChar, titleString.Length)}{boardChar}");
//            return result;
//        }

//        public override object WriteModel(WriteConfigSpreadSheetArgs configArgs, List<List<string>> data)
//        {
//            var config = configArgs as WriteStringConfigSpreadSheetArgs;
//            config = config ?? new WriteStringConfigSpreadSheetArgs();
//            return string.Join("\r\n",
//                WriteListString(data, config.AddHead, config.MaxLen, config.LineChar, config.BoardChar,
//                    config.SplitString));
//        }

//        protected override List<List<string>> ReadData(ReadSpreadSheetArgs args)
//        {
//            throw new NotImplementedException();
//        }
//    }
//}

