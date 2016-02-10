using System.Xml.Serialization;
using OfficeOpenXml;

namespace Mapper
{
    /// <summary>
	/// Description of ColumnMapping.
	/// </summary>
	public class ColumnMapping : MovableMapping
    {
        [XmlAttribute]
        public string SourceColumn { get; set; }

        public override ExcelRange GetSourceCell(int row, ExcelWorksheet worksheet)
        {
            return worksheet.Cells[row, GetSourceColumnNumber()];
        }

        public int GetSourceColumnNumber()
        {
            return ExcelHelper.ColumnLetterToInt(SourceColumn);
        }
    }
}
