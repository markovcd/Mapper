using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using OfficeOpenXml;

namespace Mapper
{
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

        public override IEnumerable<SampleEntry> GetSamples(ExcelWorkbook sourceWorkbook, DateTime date)
        {
            var sourceWorksheet = GetSourceWorksheet(sourceWorkbook);

            var from = GetSourceFromNumber();
            var to = GetSourceToNumber();

            return Enumerable.Range(from, to - from + 1)
                             .Where(i => !IsSourceEmpty(i, sourceWorksheet))
                               .Select(i => new SampleEntry(this, sourceWorksheet, date, i));
        }
    }
}
