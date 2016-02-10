using System;
using System.Collections.Generic;
using System.Linq;
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
        [XmlArrayItem(typeof(FormulaMapping))]
        public ChildItemCollection<Sample, Mapping> Mappings { get; private set; }

        [XmlAttribute]
        public string Name { get; set; }

        [XmlIgnore]
        public Card Card { get; private set; }

        public Sample()
        {
            Mappings = new ChildItemCollection<Sample, Mapping>(this);
        }
        
        public Mapping GetDateColumnMapping()
        {
        	return Mappings.FirstOrDefault(m => m.IsDateColumnMapping());
        }

	    public abstract IEnumerable<SampleEntry> GetSamples(ExcelWorkbook sourceWorkbook, DateTime date);

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
}
