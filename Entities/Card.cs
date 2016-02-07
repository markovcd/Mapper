using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using OfficeOpenXml;

namespace Mapper
{
	/// <summary>
	/// Description of Card.
	/// </summary>
	public class Card
	{
		[XmlAttribute]
		public string Name { get; set; }
	
		[XmlAttribute]
		public string TargetDateColumn { get; set; }
		
		[XmlAttribute]
		public int TargetFirstRow { get; set; }

        [XmlIgnore]
		public File File { get; set; }

        [XmlArrayItem(typeof(RowSample))]
        [XmlArrayItem(typeof(ColumnSample))]
        [XmlArrayItem(typeof(DateSheetSample))]
        public List<Sample> Samples { get; set; }
	
		
		public ExcelWorksheet GetTargetWorksheet(ExcelWorkbook workbook)
		{
            if (workbook.Worksheets[Name] == null)
                throw new KeyNotFoundException(string.Format("Nie znaleziono karty {0} w pliku wyjściowym.", Name));

            return workbook.Worksheets[Name];
		}
		
		public Card()
		{
			TargetFirstRow = 6;
			TargetDateColumn = "A";
		}
		
		public ExcelRange GetDateCell(int row, ExcelWorksheet worksheet)
		{
			return worksheet.Cells[row, GetTargetDateColumnNumber()];
		}

        public int GetTargetDateColumnNumber()
        {
            return ExcelHelper.ColumnLetterToInt(TargetDateColumn);
        }

        public bool IsTargetRowEmpty(int row, ExcelWorksheet worksheet)
        {
            return !ExcelHelper.IsValuePresent(GetDateCell(row, worksheet));
        }
    }
}
