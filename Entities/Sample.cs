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
		public string SourceCardName { get; set; }		
	
		[XmlIgnore]
		public Card Card { get; set; }

        public int GetSourceFromNumber()
        {
            return ExcelHelper.ColumnLetterToInt(SourceFrom);
        }

        public int GetSourceToNumber()
        {
            return ExcelHelper.ColumnLetterToInt(SourceTo);
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
			
		public string GetSourceCardName()
		{
			return string.IsNullOrEmpty(SourceCardName) ? Card.Name : SourceCardName;
		}
		
		public ExcelWorksheet GetSourceWorksheet(ExcelWorkbook workbook)
		{		
			if (workbook.Worksheets[GetSourceCardName()] == null)
                throw new KeyNotFoundException(string.Format("Nie znaleziono karty {0} w pliku wejściowym.", Name));

			return workbook.Worksheets[GetSourceCardName()];
		}
	
        public SampleType GetSampleType()
		{
		    var rowMappingCount = Mappings.OfType<RowMapping>().Count();
            var columnMappingCount = Mappings.OfType<ColumnMapping>().Count();

		    if (rowMappingCount > 0 && columnMappingCount == 0) return SampleType.Row;
            if (columnMappingCount > 0 && rowMappingCount == 0) return SampleType.Column;

            throw new InvalidOperationException("Nie można łączyć odwołań kolumnowych i wierszowych.");
		}
        
        #region Equals and GetHashCode implementation
        public override int GetHashCode()
		{
			int hashCode = 0;
			
			unchecked
			{
				hashCode += 1000000007 * Card.Name.GetHashCode();
				hashCode += 1000000009 * Name.GetHashCode();
			}
			
			return hashCode;
		}
        
		public override bool Equals(object obj)
		{
			Sample other = obj as Sample;
			if (other == null) return false;
			return GetHashCode().Equals(other.GetHashCode());
		}
        
		public static bool operator ==(Sample lhs, Sample rhs)
		{
			if (ReferenceEquals(lhs, rhs))
				return true;
			if (ReferenceEquals(lhs, null) || ReferenceEquals(rhs, null))
				return false;
			return lhs.Equals(rhs);
		}
        
		public static bool operator !=(Sample lhs, Sample rhs)
		{
			return !(lhs == rhs);
		}
        #endregion
	}
}
