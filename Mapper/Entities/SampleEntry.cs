using System;
using System.Collections.Generic;
using System.Linq;
using OfficeOpenXml;

namespace Mapper.Entities
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
            {
            	var date2 = new MappingEntry(dateMapping, sourceWorksheet, index).ToDate();
            	if (date2 != DateTime.MinValue) date = date2;
            }

            Sample = sample;
            Date = date;
        }
    }
}
