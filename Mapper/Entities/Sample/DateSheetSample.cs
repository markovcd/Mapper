using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using Mapper.Utilities;
using OfficeOpenXml;

namespace Mapper.Entities
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

        public override IEnumerable<SampleEntry> GetSamples(ExcelWorkbook sourceWorkbook, DateTime date)
        {
            var from = new DateTime(date.Year, date.Month, 1);
            var to = new DateTime(date.Year, date.Month, DateTime.DaysInMonth(date.Year, date.Month));

            return new DateEnumerable(Period.Daily, from, to).Where(d => !IsSourceEmpty(d, sourceWorkbook))
                                                             .Select(d => 
                                                                new SampleEntry(this, GetSourceWorksheet(d, sourceWorkbook), d, -1));
        }
    }
}
