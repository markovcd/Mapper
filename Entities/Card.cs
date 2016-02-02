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
		public string /*Target*/Name { get; set; }
	
	  //[XmlAttribute]
		//public string SourceName { get; set; }
	
		[XmlAttribute]
		public string TargetDateColumn { get; set; }
		
		[XmlAttribute]
		public int TargetFirstRow { get; set; }

        [XmlAttribute]
        public bool AddDateSeparator { get; set; }

        [XmlIgnore]
		public File File { get; set; }

        public List<Sample> Samples { get; set; }
	
		public ExcelWorksheet GetInputWorksheet(ExcelWorkbook workbook)
		{
            return GetWorksheet(workbook, Name);
		}
		
		/*GetSourceWorksheet(ExcelWorkbook workbook)*/
		/*GetTargetWorksheet(ExcelWorkbook workbook)*/
		
		public static ExcelWorksheet GetWorksheet(ExcelWorkbook workbook, string name)
		{
            if (workbook.Worksheets[name] == null)
                throw new KeyNotFoundException(string.Format("Nie znaleziono karty {0}.", name));

            return workbook.Worksheets[name];
		}
		
		public Card()
		{
			TargetFirstRow = 6;
			TargetDateColumn = "A";
		    AddDateSeparator = true;
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
