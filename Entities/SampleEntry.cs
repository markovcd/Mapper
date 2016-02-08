using System;
using System.Collections.Generic;
using System.Linq;
using OfficeOpenXml;

namespace Mapper
{
    public class SampleEntry
    {
        public Sample Sample { get; private set; }
        public DateTime Date { get; private set; }
        public IEnumerable<MappingEntry> Entries { get; private set; }

        public SampleEntry(Sample sample, ExcelWorksheet sourceWorksheet, DateTime date, int index)
        {
            Entries = sample.Mappings.Select(m => new MappingEntry(m, sourceWorksheet, index));

            var dateMapping = sample.GetDateColumnMapping();
            if (dateMapping != null)
                date = ExcelHelper.ToDate(new MappingEntry(dateMapping, sourceWorksheet, index).Value);

            Sample = sample;
            Date = date;
        }
    }
}
