using System;

namespace SegaCyrus.ToolKits.Model.SpreadSheet.Read
{
    /// <summary>
    /// 电子表格读取通用参数
    /// </summary>
    public class ReadSpreadSheetArgs
    {
        /// <summary>
        /// 电子表格数据
        /// </summary>
        protected object SpreadSheetData { get; set; }

        /// <summary>
        /// 是否携带头
        /// </summary>
        public bool HasHead { get; set; }

        /// <summary>
        /// 操作类型
        /// </summary>
        internal int TypeEnum { get; private set; }

        /// <summary>
        /// 是否使用表达式树代替反射操作
        /// </summary>
        //[Obsolete("实验性功能,开启后将使用表达式树代替反射进行属性赋值")]
        public bool UseExpressTree { get; set; }

        /// <summary>
        /// 电子表格读取通用参数
        /// </summary>
        /// <param name="spreadSheetData">电子表格数据</param>
        /// <param name="hasHead">是否携带头</param>
        /// <param name="type">类型</param>
        public ReadSpreadSheetArgs(object spreadSheetData, bool hasHead, int type)
        {
            SpreadSheetData = spreadSheetData;
            TypeEnum = type;
            HasHead = hasHead;
        }
    }
}