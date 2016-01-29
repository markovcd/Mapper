using System;
using System.Linq;
using System.Collections.Generic;
using System.Xml.Serialization;
using OfficeOpenXml;

namespace Mapper
{
    public enum SampleType { Row, Column }

    /// <summary>
    /// Description of Sample.
    /// </summary>
    public class Sample
	{
		public List<Mapping> Mappings { get; set; }
	
		[XmlAttribute]
		public string Name { get; set; }
	
		[XmlAttribute]
		public string SourceFrom { get; set; }
	
		[XmlAttribute]
		public string SourceTo { get; set; }
	
		[XmlAttribute]
		public int Page { get; set; }		
	
		[XmlIgnore]
		public Card Card { get; set; }

        public Sample()
		{
			Page = 1;
		}

        public int GetSourceFromNumber()
        {
            return ExcelHelper.ColumnLetterToInt(SourceFrom);
        }

        public int GetSourceToNumber()
        {
            return ExcelHelper.ColumnLetterToInt(SourceTo);
        }

        public string GetIdentifier()
		{
			return string.Format("{0}.{1}", GetFullCardName(), Name);
		}
		
		public bool IsSourceEmpty(int index, ExcelWorksheet worksheet)
		{	
			switch (GetSampleType())
			{
			    case SampleType.Column:
			        return IsColumnSourceEmpty(index, worksheet);
			    case SampleType.Row:
			        return IsRowSourceEmpty(index, worksheet);
			}

		    throw new InvalidOperationException();
		}
		
		private bool IsRowSourceEmpty(int column, ExcelWorksheet worksheet)
		{
			return Mappings.OfType<RowMapping>()
						   .Select(m => m.IsSourceValuePresent(column, worksheet))
						   .All(b => !b);			
		}
		
		private bool IsColumnSourceEmpty(int row, ExcelWorksheet worksheet)
		{
			return Mappings.OfType<ColumnMapping>()
			               .Select(m => m.IsSourceValuePresent(row, worksheet))
						   .All(b => !b);			
		}
		
		public ExcelWorksheet GetOutputWorksheet(ExcelWorkbook workbook)
		{
			return workbook.Worksheets[GetFullCardName()];
		}
	
		public string GetFullCardName()
		{
		    return Page == 1 ? Card.Name : string.Format("{0}_{1}", Card.Name, Page);
		}

        public SampleType GetSampleType()
		{
		    var rowMappingCount = Mappings.OfType<RowMapping>().Count();
            var columnMappingCount = Mappings.OfType<ColumnMapping>().Count();

		    if (rowMappingCount > 0 && columnMappingCount == 0) return SampleType.Row;
            if (columnMappingCount > 0 && rowMappingCount == 0) return SampleType.Column;

            throw new InvalidOperationException("Nie można łączyć odwołań kolumnowych i wierszowych.");
		}
	}
}
