using System;
using System.Collections.Generic;

namespace Mapper
{
    public class SampleEntry
    {
        public Sample Sample { get; private set; }
        public DateTime Date { get; private set; }
        public IEnumerable<MappingEntry> Entries { get; private set; }

        public SampleEntry(Sample sample, DateTime date, IEnumerable<MappingEntry> entries)
        {
            Sample = sample;
            Date = date;
            Entries = entries;
        }
    }
}
