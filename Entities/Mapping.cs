using System;
using System.Xml.Serialization;
using OfficeOpenXml;

namespace Mapper
{
    

    public abstract class Mapping : IChildItem<Sample>
    {
		[XmlAttribute]
		public string Caption { get; set; }
	
		[XmlAttribute]
		public string TargetColumn { get; set; }

        [XmlAttribute]
        public bool IsIgnorable { get; set; }

        [XmlIgnore]
		public Sample Sample { get; private set; }
	
		public ExcelRange GetTargetCell(int row, ExcelWorksheet worksheet)
		{		   
			return worksheet.Cells[row, GetTargetColumnNumber()];
		}
		
		public int GetTargetColumnNumber()
		{
			return ExcelHelper.ColumnLetterToInt(TargetColumn);
		}

	    public bool IsDateColumnMapping()
	    {
	        return GetTargetColumnNumber() == Sample.Card.GetTargetDateColumnNumber();
	    }

        #region IChildItem<Sample> Members

        Sample IChildItem<Sample>.Parent
        {
            get { return Sample; }
            set { Sample = value; }
        }

        #endregion
    }
}
