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
        public List<string> DateFormats { get; set; }
        
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
        
        private IEnumerable<string> targetColumns;
        
        [XmlIgnore]
        public IEnumerable<string> TargetColumns
        {
        	get
        	{
        		if (targetColumns == null) targetColumns = GetTargetColumns();
        		return targetColumns;
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
            
            DateFormats = new List<string> 
            {
            	"yyyy-MM-dd", "dd-MM-yyyy", "dd.MM.yyyy", "dd,MM,yyyy", "dd.MM,yyyy", 
            	"dd,MM.yyyy", "dd MM yyyy", "yyyy.MM.dd", "d.MM.yyyy"
            };
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
        
        private IEnumerable<string> GetTargetColumns()
        {
        	return Samples.SelectMany(s => s.Mappings)
        		          .Select(m => m.TargetColumn)
        		          .Concat(new[] {TargetDateColumn})
        		          .Distinct();
        }

        public Dictionary<Sample, IEnumerable<SampleEntry>> GetCard(ExcelWorkbook sourceWorkbook, DateTime date)
        {
            Func<IGrouping<Sample, SampleEntry>, IEnumerable<SampleEntry>> sort =
                g => Order == Order.ByDates ? g.OrderBy(s => s.Date).AsEnumerable() : g.AsEnumerable();

            return Samples.SelectMany(s => s.GetSamples(sourceWorkbook, date))
                          .GroupBy(s => s.Sample)
                          .ToDictionary(g => g.Key, sort);
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
