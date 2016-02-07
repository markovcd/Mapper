using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using OfficeOpenXml;

namespace Mapper
{
	/// <summary>
	/// Description of Card.
	/// </summary>
	public class Card : IChildItem<File>
	{
		[XmlAttribute]
		public string Name { get; set; }
	
		[XmlAttribute]
		public string TargetDateColumn { get; set; }
		
		[XmlAttribute]
		public int TargetFirstRow { get; set; }

        [XmlIgnore]
		public File File { get; internal set; }

        [XmlArrayItem(typeof(RowSample))]
        [XmlArrayItem(typeof(ColumnSample))]
        [XmlArrayItem(typeof(DateSheetSample))]
        public ChildItemCollection<Card, Sample> Samples { get; private set; }
	
		
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
            Samples = new ChildItemCollection<Card, Sample>(this);

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

        #region IChildItem<File> Members

        File IChildItem<File>.Parent
        {
            get { return File; }
            set { File = value; }
        }

        #endregion
    }
}
