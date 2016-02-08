using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using OfficeOpenXml;

namespace Mapper
{
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

        public bool IsSourceEmpty(DateTime date, ExcelWorkbook workbook)
        {
            return Mappings.OfType<CellMapping>()
                           .Select(m => m.IsSourceValuePresent(GetSourceWorksheet(date, workbook)))
                           .All(b => !b);
        }
    }
}
