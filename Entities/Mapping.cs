using System.Xml.Serialization;
using OfficeOpenXml;

namespace Mapper
{
	/// <summary>
	/// Description of CellMapping.
	/// </summary>
	public class CellMapping : Mapping
	{
		[XmlAttribute]
		public string SourceAddress { get; set; }
		
		public ExcelRange GetSourceCell(ExcelWorksheet worksheet)
		{
			return worksheet.Cells[SourceAddress];
		}
		
		public object GetValue(ExcelWorksheet worksheet)
		{	
			return GetSourceCell(worksheet).Value;
		}

        public bool IsSourceValuePresent(ExcelWorksheet worksheet)
        {
            return !IsIgnorable && ExcelHelper.IsValuePresent(GetSourceCell(worksheet));
        }
    }
	
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

    public abstract class MovableMapping : Mapping
    {
       
        public abstract ExcelRange GetSourceCell(int index, ExcelWorksheet worksheet);

        public object GetValue(int index, ExcelWorksheet worksheet)
        {
            return GetSourceCell(index, worksheet).Value;
        }

        public bool IsSourceValuePresent(int index, ExcelWorksheet worksheet)
        {
            return !IsIgnorable && ExcelHelper.IsValuePresent(GetSourceCell(index, worksheet));
        }
    }

    public class ContentMapping : Mapping
	{
		[XmlAttribute]
		public string Content { get; set; }
		
		public string GetValue()
		{
			return Content;
		}
	}
	
	public abstract class Mapping
	{
		[XmlAttribute]
		public string Caption { get; set; }
	
		[XmlAttribute]
		public string TargetColumn { get; set; }

        [XmlAttribute]
        public bool IsIgnorable { get; set; }

        [XmlIgnore]
		public Sample Sample { get; set; }
	
		public ExcelRange GetTargetCell(int row, ExcelWorksheet worksheet)
		{		   
			return worksheet.Cells[row, GetTargetColumnNumber()];
		}
		
		public int GetTargetColumnNumber()
		{
			return ExcelHelper.ColumnLetterToInt(TargetColumn);
		}
	}
}
