using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using OfficeOpenXml;

namespace Mapper
{
	public enum Order { BySamples, ByDates }
    
    /// <summary>
	/// Description of Card.
	/// </summary>
	public class Card : IChildItem<File>
    {
        private int firstColumnNumber, lastColumnNumber;

        [XmlAttribute]
		public string Name { get; set; }
	
		[XmlAttribute]
		public string TargetDateColumn { get; set; }
		
		[XmlAttribute]
		public int TargetFirstRow { get; set; }

        [XmlAttribute]
        public Order Order { get; set; }

        [XmlIgnore]
		public File File { get; internal set; }

        [XmlIgnore]
        public int FirstColumnNumber
        {
            get
            {
                if (firstColumnNumber == 0)
                {
                    var t = GetTargetColumnRange();
                    firstColumnNumber = t.Item1;
                    lastColumnNumber = t.Item2;
                }

                return firstColumnNumber;
            }
        }

        [XmlIgnore]
        public int LastColumnNumber
        {
            get
            {
                if (lastColumnNumber == 0)
                {
                    var t = GetTargetColumnRange();
                    firstColumnNumber = t.Item1;
                    lastColumnNumber = t.Item2;
                }

                return lastColumnNumber;
            }
        }

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

        private Tuple<int, int> GetTargetColumnRange()
        {
            var columns = Samples.SelectMany(s => s.Mappings)
                                 .Select(m => m.GetTargetColumnNumber())
                                 .Concat(new[] {GetTargetDateColumnNumber()})
                                 .OrderBy(i => i).ToList();

            return Tuple.Create(columns.First(), columns.Last());
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
