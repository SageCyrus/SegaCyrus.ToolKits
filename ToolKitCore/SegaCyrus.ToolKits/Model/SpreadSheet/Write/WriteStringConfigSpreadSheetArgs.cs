using SegaCyrus.ToolKits.Model.SpreadSheet.Enums;

namespace SegaCyrus.ToolKits.Model.SpreadSheet.Write
{
    /// <summary>
    /// 格式化列表参数
    /// </summary>
    public class WriteStringConfigSpreadSheetArgs : WriteConfigSpreadSheetArgs
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="addHead">是否携带表头</param>
        /// <param name="truncateText">是否截断数据</param>
        /// <param name="withHorizontalEdge">是否携带数据分界线</param>
        /// <param name="maxCellSize">最长长度</param>
        /// <param name="topLeftVertex">左上顶点</param>
        /// <param name="topRightVertex">右上顶点</param>
        /// <param name="bottomLeftVertex">左下顶点</param>
        /// <param name="bottomRightVertex">右下顶点</param>
        /// <param name="topSpliteVertexEdge">顶部边线</param>
        /// <param name="bottomSpliteVertexEdge">底部边线</param>
        /// <param name="leftSpliteVertexEdge">左边线</param>
        /// <param name="middleSpliteVertexEdge">中间拆分线</param>
        /// <param name="rightSpliteVertexEdge">右边线</param>
        /// <param name="horizontalEdge">水平边线</param>
        /// <param name="verticalEdge">垂直边线</param>
        public WriteStringConfigSpreadSheetArgs(bool addHead = true, bool truncateText = true,
            bool withHorizontalEdge = false, int maxCellSize = 38,
            string topLeftVertex = "┌", string topRightVertex = "┐", string bottomLeftVertex = "└",
            string bottomRightVertex = "┘",
            string topSpliteVertexEdge = "┬", string bottomSpliteVertexEdge = "┴", string leftSpliteVertexEdge = "├",
            string middleSpliteVertexEdge = "┼",
            string rightSpliteVertexEdge = "┤", char horizontalEdge = '─', string verticalEdge = "│") : base(
            SpreadSheetTypeEnum.FormatString, addHead)
        {
            TruncateDataLen = maxCellSize;
            TopLeftVertex = topLeftVertex;
            TopRightVertex = topRightVertex;
            BottomLeftVertex = bottomLeftVertex;
            BottomRightVertex = bottomRightVertex;
            TopSpliteVertexEdge = topSpliteVertexEdge;
            BottomSpliteVertexEdge = bottomSpliteVertexEdge;
            LeftSpliteVertexEdge = leftSpliteVertexEdge;
            MiddleSpliteVertexEdge = middleSpliteVertexEdge;
            RightSpliteVertexEdge = rightSpliteVertexEdge;
            HorizontalEdge = horizontalEdge;
            VerticalEdge = verticalEdge;
            WithHorizontalEdge = withHorizontalEdge;
            TruncateText = truncateText;
        }


        ///// <summary>
        ///// 行分界符
        ///// </summary>
        //public char LineChar { get; set; }

        ///// <summary>
        ///// 边界符
        ///// </summary>
        //public char BoardChar { get; set; }


        ///// <summary>
        ///// 列符
        ///// </summary>
        //public string SplitString { get; set; }

        /// <summary>
        /// 每个单元格字符最长长度
        /// </summary>

        public int TruncateDataLen { get; set; }

        /// <summary>
        /// 左顶部顶点
        /// </summary>

        public string TopLeftVertex { get; set; }

        /// <summary>
        /// 右侧顶点
        /// </summary>
        public string TopRightVertex { get; set; }

        /// <summary>
        /// 底部左侧顶点
        /// </summary>
        public string BottomLeftVertex { get; set; }

        /// <summary>
        /// 底部顶点
        /// </summary>
        public string BottomRightVertex { get; set; }

        /// <summary>
        /// 顶部边线
        /// </summary>
        public string TopSpliteVertexEdge { get; set; }

        /// <summary>
        /// 底部边线
        /// </summary>
        public string BottomSpliteVertexEdge { get; set; }

        /// <summary>
        /// 左侧边线
        /// </summary>
        public string LeftSpliteVertexEdge { get; set; }

        /// <summary>
        /// 中间十字边线
        /// </summary>
        public string MiddleSpliteVertexEdge { get; set; }

        /// <summary>
        /// 右侧边线
        /// </summary>
        public string RightSpliteVertexEdge { get; set; }

        /// <summary>
        /// 水平分界线
        /// </summary>
        public char HorizontalEdge { get; set; }

        /// <summary>
        /// 垂直分界线
        /// </summary>
        public string VerticalEdge { get; set; }

        /// <summary>
        /// 是否携带数据间分界线
        /// </summary>
        public bool WithHorizontalEdge { get; set; }

        /// <summary>
        /// 是否截断数据
        /// </summary>
        public bool TruncateText { get; set; }
    }
}