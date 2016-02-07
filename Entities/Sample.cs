using System;
using System.Linq;
using System.Collections.Generic;
using System.Xml.Serialization;
using OfficeOpenXml;

namespace Mapper
{
    public abstract class Sample : IChildItem<Card>
    {
        [XmlArrayItem(typeof(RowMapping))]
        [XmlArrayItem(typeof(ColumnMapping))]
        [XmlArrayItem(typeof(CellMapping))]
        [XmlArrayItem(typeof(ContentMapping))]
        public ChildItemCollection<Sample, Mapping> Mappings { get; private set; }

        [XmlAttribute]
        public string Name { get; set; }

        [XmlIgnore]
        public Card Card { get; private set; }

        public Sample()
        {
            Mappings = new ChildItemCollection<Sample, Mapping>(this);
        }

        #region Equals and GetHashCode implementation
        public override int GetHashCode()
        {
            var hashCode = 0;

            unchecked
            {
                hashCode += 1000000007 * Card.Name.GetHashCode();
                hashCode += 1000000009 * Name.GetHashCode();
            }

            return hashCode;
        }

        public override bool Equals(object obj)
        {
            var other = obj as Sample;
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

        #region IChildItem<Card> Members

        Card IChildItem<Card>.Parent
        {
            get { return Card; }
            set { Card = value; }
        }

        #endregion
    }

    public class DateSheetSample : Sample
    {
        [XmlAttribute]
        public string DateFormat { get; set; }

        public string GetSourceCardName(DateTime date)
        {
            return date.ToString(DateFormat);
        }

        public ExcelWorksheet GetSourceWorksheet(DateTime date, ExcelWorkbook workbook)
        {
            var worksheet = workbook.Worksheets[GetSourceCardName(date)];
            if (worksheet == null)
                throw new KeyNotFoundException(string.Format("Nie znaleziono karty {0} w pliku wejściowym.", GetSourceCardName(date)));

            return worksheet;
        }

        public bool IsSourceEmpty(ExcelWorksheet worksheet)
        {
            return Mappings.OfType<CellMapping>()
                           .Select(m => m.IsSourceValuePresent(worksheet))
                           .All(b => !b);
        }
    }

    public abstract class SingleSheetSample : Sample
    {
        [XmlAttribute]
        public string SourceCardName { get; set; }

        [XmlAttribute]
        public string SourceFrom { get; set; }

        [XmlAttribute]
        public string SourceTo { get; set; }

        public string GetSourceCardName()
        {
            return string.IsNullOrEmpty(SourceCardName) ? Card.Name : SourceCardName;
        }

        public abstract bool IsSourceEmpty(int index, ExcelWorksheet worksheet);
        public abstract int GetSourceFromNumber();
        public abstract int GetSourceToNumber();       

        public ExcelWorksheet GetSourceWorksheet(ExcelWorkbook workbook)
        {
            var worksheet = workbook.Worksheets[GetSourceCardName()];
            if (worksheet == null)
                throw new KeyNotFoundException(string.Format("Nie znaleziono karty {0} w pliku wejściowym.", GetSourceCardName()));

            return worksheet;
        }
    }

    public class ColumnSample : SingleSheetSample
    {
        public override int GetSourceFromNumber()
        {
            int i;
            if (int.TryParse(SourceFrom, out i)) return i;
            throw new FormatException("Wartość w SourceFrom w ColumnSample musi być liczbą.");
        }

        public override int GetSourceToNumber()
        {
            int i;
            if (int.TryParse(SourceTo, out i)) return i;
            throw new FormatException("Wartość w SourceTo w ColumnSample musi być liczbą.");
        }

        public override bool IsSourceEmpty(int row, ExcelWorksheet worksheet)
        {
            return Mappings.OfType<ColumnMapping>()
                           .Select(m => m.IsSourceValuePresent(row, worksheet))
                           .All(b => !b);
        }
    }

    /// <summary>
    /// Description of RowSample.
    /// </summary>
    public class RowSample : SingleSheetSample
    {			
        public override int GetSourceFromNumber()
        {
            return ExcelHelper.ColumnLetterToInt(SourceFrom);
        }

        public override int GetSourceToNumber()
        {
            return ExcelHelper.ColumnLetterToInt(SourceTo);
        }

        public override bool IsSourceEmpty(int column, ExcelWorksheet worksheet)
		{
			return Mappings.OfType<RowMapping>()
						   .Select(m => m.IsSourceValuePresent(column, worksheet))
						   .All(b => !b);			
		}
	}
}
