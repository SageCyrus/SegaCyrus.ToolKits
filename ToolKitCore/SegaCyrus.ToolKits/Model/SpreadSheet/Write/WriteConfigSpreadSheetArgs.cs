using System;
using System.Collections.Generic;
using SegaCyrus.ToolKits.Model.SpreadSheet.Enums;

namespace SegaCyrus.ToolKits.Model.SpreadSheet.Write
{
    /// <summary>
    /// 电子表格通用写入类
    /// </summary>
    public abstract class WriteConfigSpreadSheetArgs
    {
        /// <summary>
        /// 是否携带表头
        /// </summary>
        public bool AddHead { get; set; }

        /// <summary>
        /// 操作类型
        /// </summary>
        public int TypeEnum { get; set; }

        /// <summary>
        /// 是否使用表达式树代替反射操作
        /// </summary>
        //[Obsolete("实验性功能,开启后将使用表达式树代替反射进行属性赋值")]
        public bool UseExpressTree{get;set;}


        /// <summary>
        /// 电子表格写入类
        /// </summary>
        /// <param name="type">操作类型</param>
        /// <param name="addHead">是否携带表头</param>
        public WriteConfigSpreadSheetArgs(SpreadSheetTypeEnum type, bool addHead = true)
        {
            TypeEnum = (int)type;
            AddHead = addHead;
        }

        /// <summary>
        /// 电子表格写入类
        /// </summary>
        /// <param name="type">操作类型</param>
        /// <param name="addHead">是否携带表头</param>
        public WriteConfigSpreadSheetArgs(int type, bool addHead = true)
        {
            TypeEnum = type;
            AddHead = addHead;
        }

        /// <summary>
        /// 调整Column的列序,若新增不存在的列则扩无值列
        /// </summary>
        /// <param name="column">当前的列序</param>
        /// <returns>排序后的列序</returns>
        public virtual List<string> ColumnOrder(List<string> column)
        {
            return column;
        }
    }
}