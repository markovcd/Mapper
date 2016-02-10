using System.Xml.Serialization;
using OfficeOpenXml;

namespace Mapper
{
    /// <summary>
	/// Description of RowMapping.
	/// </summary>
	public class RowMapping : MovableMapping
    {
        [XmlAttribute]
        public int SourceRow { get; set; }

        public override ExcelRange GetSourceCell(int column, ExcelWorksheet worksheet)
        {
            return worksheet.Cells[SourceRow, column];
        }
    }
}
