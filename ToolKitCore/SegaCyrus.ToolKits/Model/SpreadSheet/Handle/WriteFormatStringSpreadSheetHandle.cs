using System.IO;
using System.Text;
using SegaCyrus.ToolKits.Model.SpreadSheet.Append;
using SegaCyrus.ToolKits.Model.SpreadSheet.Enums;

namespace SegaCyrus.ToolKits.Model.SpreadSheet.Handle
{
    class WriteFormatStringSpreadSheetHandle : WriteSpreadSheetHandle
    {
        public WriteFormatStringSpreadSheetHandle() : base(SpreadSheetTypeEnum.FormatString)
        {
            Handle = new StringBuilder();
        }

        public override byte[] GetBytes()
        {
            throw new System.NotImplementedException();
        }

        public override Stream WriteStream(Stream stream)
        {
            throw new System.NotImplementedException();
        }
    }
}